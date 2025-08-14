using Moq;
using FluentValidation;
using FluentValidation.Results;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.UnitTests.Services;

[TestFixture]
public class OrganizationServiceTests
{
    private Mock<IOrganizationRepository> _organizationRepository;
    private Mock<IValidator<CreateOrganizationDto>> _createValidator;
    private Mock<IValidator<UpdateOrganizationDto>> _updateValidator;
    private Mock<ISlugGenerator> _slugGenerator;
    private OrganizationService _service;

    [SetUp]
    public void SetUp()
    {
        _organizationRepository = new Mock<IOrganizationRepository>();
        _createValidator = new Mock<IValidator<CreateOrganizationDto>>();
        _updateValidator = new Mock<IValidator<UpdateOrganizationDto>>();
        _slugGenerator = new Mock<ISlugGenerator>();
        _service = new OrganizationService(_organizationRepository.Object, _createValidator.Object, _slugGenerator.Object, _updateValidator.Object);
    }

    [Test]
    public async Task CreateAsync_ValidDto_CreatesOrganization()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = "Test Org" };
        var now = DateTime.UtcNow;

        _createValidator
            .Setup(v => v.ValidateAsync(dto, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _slugGenerator
            .Setup(s => s.GenerateAsync<Organization>(dto.Name))
            .ReturnsAsync("test-org");

        _organizationRepository
            .Setup(r => r.CreateAsync(It.IsAny<Organization>()))
            .ReturnsAsync((Organization o) => o);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert (state)
        Assert.That(result.Name, Is.EqualTo(dto.Name));
        Assert.That(result.Slug, Is.EqualTo("test-org"));

        // Assert (behavior)
        _createValidator.Verify(v => v.ValidateAsync(dto, CancellationToken.None), Times.Once);
        _slugGenerator.Verify(s => s.GenerateAsync<Organization>(dto.Name), Times.Once);
        _organizationRepository.Verify(r => r.CreateAsync(
            It.Is<Organization>(o => o.Name == "Test Org" && o.Slug == "test-org")), Times.Once);
    }

    [Test]
    public void UpdateAsync_OrganizationNotFound_ThrowsNotFoundException()
    {
        var dto = new UpdateOrganizationDto { Id = 99, Name = "New Name" };
        var validationResult = new ValidationResult();
        _updateValidator.Setup(v => v.ValidateAsync(dto, CancellationToken.None)).ReturnsAsync(validationResult);
        _organizationRepository.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Organization)null!);

        Assert.ThrowsAsync<NotFoundException>(async () => await _service.UpdateAsync(dto));
    }

    [Test]
    public async Task GetByIdAsync_OrganizationExists_ReturnsDto()
    {
        var org = new Organization { Id = 1, Name = "Test Org", Slug = "test-org", CreatedAt = DateTime.UtcNow };
        _organizationRepository.Setup(r => r.GetByIdAsync(org.Id)).ReturnsAsync(org);

        var result = await _service.GetByIdAsync(org.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(org.Id));
        Assert.That(result.Name, Is.EqualTo(org.Name));
    }

    [Test]
    public void DeleteAsync_OrganizationNotFound_ThrowsNotFoundException()
    {
    _organizationRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Organization)null!);
        Assert.ThrowsAsync<NotFoundException>(async () => await _service.DeleteAsync(99));
    }

    [Test]
    public async Task DeleteAsync_OrganizationExists_DeletesOrganization()
    {
        var org = new Organization { Id = 1, Name = "Test Org", Slug = "test-org", CreatedAt = DateTime.UtcNow };
        _organizationRepository.Setup(r => r.GetByIdAsync(org.Id)).ReturnsAsync(org);
        _organizationRepository.Setup(r => r.DeleteAsync(org.Id)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(org.Id);
        _organizationRepository.Verify(r => r.DeleteAsync(org.Id), Times.Once);
    }
}

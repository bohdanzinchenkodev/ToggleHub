using FluentValidation.TestHelper;
using Moq;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.Validators;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.UnitTests.Validators;

[TestFixture]
public class UpdateOrganizationValidatorTests
{
    private Mock<IOrganizationRepository> _mockRepository;
    private UpdateOrganizationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IOrganizationRepository>();
        _validator = new UpdateOrganizationValidator(_mockRepository.Object);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 1, Name = "" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Organization name is required");
    }

    [Test]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 1, Name = null! };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsTooLong()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 1, Name = new string('a', 101) };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Organization name must be between 1 and 100 characters");
    }

    [Test]
    public async Task Should_HaveError_When_IdIsInvalid()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 0, Name = "Valid Name" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Organization ID must be greater than 0");
    }

    [Test]
    public async Task Should_HaveError_When_NameAlreadyExists_ForDifferentOrganization()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 1, Name = "Existing Org" };
        _mockRepository.Setup(r => r.NameExistsAsync("Existing Org", 1))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("An organization with this name already exists");
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsValid()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 1, Name = "Valid Organization" };
        _mockRepository.Setup(r => r.NameExistsAsync("Valid Organization", 1))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
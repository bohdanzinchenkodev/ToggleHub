using FluentValidation.TestHelper;
using Moq;
using ToggleHub.Application.Validators;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.UnitTests.Validators;

[TestFixture]
public class OrganizationValidatorTests
{
    private Mock<IOrganizationRepository> _mockRepository;
    private OrganizationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IOrganizationRepository>();
        _validator = new OrganizationValidator(_mockRepository.Object);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var organization = new Organization { Name = "" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(organization);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Organization name is required");
    }

    [Test]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var organization = new Organization { Name = null! };

        // Act & Assert
        var result = await _validator.TestValidateAsync(organization);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsTooLong()
    {
        // Arrange
        var organization = new Organization { Name = new string('a', 101) };

        // Act & Assert
        var result = await _validator.TestValidateAsync(organization);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Organization name must be between 1 and 100 characters");
    }

    [Test]
    public async Task Should_HaveError_When_NameAlreadyExists_ForNewOrganization()
    {
        // Arrange
        var organization = new Organization { Id = 0, Name = "Existing Org" };
        _mockRepository.Setup(r => r.NameExistsAsync("Existing Org"))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(organization);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("An organization with this name already exists");
    }

    [Test]
    public async Task Should_HaveError_When_NameAlreadyExists_ForExistingOrganization()
    {
        // Arrange
        var organization = new Organization { Id = 1, Name = "Existing Org" };
        _mockRepository.Setup(r => r.NameExistsAsync("Existing Org", 1))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(organization);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("An organization with this name already exists");
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsValid_ForNewOrganization()
    {
        // Arrange
        var organization = new Organization { Id = 0, Name = "Valid Organization" };
        _mockRepository.Setup(r => r.NameExistsAsync("Valid Organization"))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(organization);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsValid_ForExistingOrganization()
    {
        // Arrange
        var organization = new Organization { Id = 1, Name = "Valid Organization" };
        _mockRepository.Setup(r => r.NameExistsAsync("Valid Organization", 1))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(organization);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}

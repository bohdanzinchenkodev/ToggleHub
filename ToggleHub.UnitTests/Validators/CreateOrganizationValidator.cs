using FluentValidation.TestHelper;
using Moq;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Validators;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.UnitTests.Validators;

[TestFixture]
public class CreateOrganizationValidatorTests
{
    private Mock<IOrganizationRepository> _mockRepository;
    private CreateOrganizationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IOrganizationRepository>();
        _validator = new CreateOrganizationValidator(_mockRepository.Object);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = "" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Organization name is required");
    }

    [Test]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = null! };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsTooLong()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = new string('a', 101) };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Organization name must be between 1 and 100 characters");
    }

    [Test]
    public async Task Should_HaveError_When_NameAlreadyExists()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = "Existing Org" };
        _mockRepository.Setup(r => r.NameExistsAsync("Existing Org"))
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
        var dto = new CreateOrganizationDto { Name = "Valid Organization" };
        _mockRepository.Setup(r => r.NameExistsAsync("Valid Organization"))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}


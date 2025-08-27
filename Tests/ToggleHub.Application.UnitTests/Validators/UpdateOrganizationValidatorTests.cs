using FluentValidation.TestHelper;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Validators;

namespace ToggleHub.Application.UnitTests.Validators;

[TestFixture]
public class UpdateOrganizationValidatorTests
{
    private UpdateOrganizationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateOrganizationValidator();
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
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_IdIsInvalid()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 0, Name = "Valid Name" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsValid()
    {
        // Arrange
        var dto = new UpdateOrganizationDto { Id = 1, Name = "Valid Organization" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
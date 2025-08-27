using FluentValidation.TestHelper;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Validators;

namespace ToggleHub.UnitTests.Validators;

[TestFixture]
public class UpdateProjectValidatorTests
{
    private UpdateProjectValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateProjectValidator();
    }

    [Test]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = null! };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsTooLong()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = new string('a', 101) };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsExactly100Characters()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = new string('a', 100) };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_IdIsZero()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 0, Name = "Valid Project" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    public async Task Should_HaveError_When_IdIsNegative()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = -1, Name = "Valid Project" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    public async Task Should_NotHaveError_When_AllPropertiesAreValid()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "Valid Project" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsSingleCharacter()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "A" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameContainsOnlyWhitespace()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "   " };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}

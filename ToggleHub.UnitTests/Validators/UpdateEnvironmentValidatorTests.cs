using FluentValidation.TestHelper;
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Validators;

namespace ToggleHub.UnitTests.Validators;

[TestFixture]
public class UpdateEnvironmentValidatorTests
{
    private UpdateEnvironmentValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateEnvironmentValidator();
    }

    [Test]
    public async Task Should_HaveError_When_IdIsZero()
    {
        // Arrange
        var dto = new UpdateEnvironmentDto { Id = 0, TypeString = "Dev" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
    
    [Test]
    public async Task Should_HaveError_When_TypeIsNull()
    {
        // Arrange
        var dto = new UpdateEnvironmentDto { Id = 1, TypeString = null! };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public async Task Should_HaveError_When_TypeIsEmpty()
    {
        // Arrange
        var dto = new UpdateEnvironmentDto { Id = 1, TypeString = "" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public async Task Should_HaveError_When_TypeIsWhitespace()
    {
        // Arrange
        var dto = new UpdateEnvironmentDto { Id = 1, TypeString = "   " };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public async Task Should_NotHaveError_When_AllPropertiesAreValid()
    {
        // Arrange
        var dto = new UpdateEnvironmentDto { Id = 1, TypeString = "Dev" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
    
}

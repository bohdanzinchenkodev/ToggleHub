using FluentValidation.TestHelper;
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Validators;

namespace ToggleHub.Application.UnitTests.Validators;

[TestFixture]
public class CreateEnvironmentValidatorTests
{
    private CreateEnvironmentValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateEnvironmentValidator();
    }

    [Test]
    public async Task Should_HaveError_When_TypeIsNull()
    {
        // Arrange
        var dto = new CreateEnvironmentDto { TypeString = null!, ProjectId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public async Task Should_HaveError_When_TypeIsEmpty()
    {
        // Arrange
        var dto = new CreateEnvironmentDto { TypeString = "", ProjectId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public async Task Should_HaveError_When_TypeIsWhitespace()
    {
        // Arrange
        var dto = new CreateEnvironmentDto { TypeString = "   ", ProjectId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public async Task Should_HaveError_When_ProjectIdIsZero()
    {
        // Arrange
        var dto = new CreateEnvironmentDto { TypeString = "Dev", ProjectId = 0 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Test]
    public async Task Should_NotHaveError_When_AllPropertiesAreValid()
    {
        // Arrange
        var dto = new CreateEnvironmentDto { TypeString = "Dev", ProjectId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

using FluentValidation.TestHelper;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Validators;

namespace ToggleHub.UnitTests.Validators;

[TestFixture]
public class CreateProjectValidatorTests
{
    private CreateProjectValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateProjectValidator();
    }

    [Test]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "", OrganizationId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = null!, OrganizationId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsTooLong()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = new string('a', 101), OrganizationId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsExactly100Characters()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = new string('a', 100), OrganizationId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_OrganizationIdIsZero()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "Valid Project", OrganizationId = 0 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationId);
    }

    [Test]
    public async Task Should_NotHaveError_When_AllPropertiesAreValid()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "Valid Project", OrganizationId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsSingleCharacter()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "A", OrganizationId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name); // EXPECT NO ERROR
    }

    [Test]
    public async Task Should_HaveError_When_NameContainsOnlyWhitespace()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "   ", OrganizationId = 1 };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}

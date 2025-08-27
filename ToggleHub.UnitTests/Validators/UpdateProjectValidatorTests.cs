using FluentValidation;
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

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = null! };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsTooLong()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = new string('a', 101) };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsExactly100Characters()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = new string('a', 100) };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.False);
    }

    [Test]
    public async Task Should_HaveError_When_IdIsZero()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 0, Name = "Valid Project" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Id"), Is.True);
    }

    [Test]
    public async Task Should_HaveError_When_IdIsNegative()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = -1, Name = "Valid Project" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Id"), Is.True);
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsValid()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "Valid Project" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.False);
    }

    [Test]
    public async Task Should_NotHaveError_When_IdIsValid()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "Valid Project" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Id"), Is.False);
    }

    [Test]
    public async Task Should_NotHaveError_When_AllPropertiesAreValid()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "Valid Project" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task Should_NotHaveError_When_NameIsSingleCharacter()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "A" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.False);
    }

    [Test]
    public async Task Should_HaveError_When_NameContainsOnlyWhitespace()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = 1, Name = "   " };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task Should_NotHaveError_When_IdIsLargePositiveNumber()
    {
        // Arrange
        var dto = new UpdateProjectDto { Id = int.MaxValue, Name = "Valid Project" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Id"), Is.False);
    }
}

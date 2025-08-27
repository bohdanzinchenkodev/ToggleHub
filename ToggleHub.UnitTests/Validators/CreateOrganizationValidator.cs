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
    private CreateOrganizationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateOrganizationValidator();
    }

    [Test]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = "" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = null! };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
    }

    [Test]
    public async Task Should_HaveError_When_NameIsTooLong()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = new string('a', 101) };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    

    [Test]
    public async Task Should_NotHaveError_When_NameIsValid()
    {
        // Arrange
        var dto = new CreateOrganizationDto { Name = "Valid Organization" };

        // Act & Assert
        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}


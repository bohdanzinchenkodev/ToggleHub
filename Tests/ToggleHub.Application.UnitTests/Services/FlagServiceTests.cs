using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.UnitTests.Services;

public class FlagServiceTests
{
    private Mock<IValidator<CreateFlagDto>> _mockCreateValidator;
    private Mock<IValidator<UpdateFlagDto>> _mockUpdateValidator;
    private Mock<IFlagRepository> _mockFlagRepository;
    private Mock<IEnvironmentRepository> _mockEnvironmentRepository;
    private Mock<IProjectRepository> _mockProjectRepository;
    private FlagService _flagService;

    [SetUp]
    public void SetUp()
    {
        _mockCreateValidator = new Mock<IValidator<CreateFlagDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateFlagDto>>();
        _mockFlagRepository = new Mock<IFlagRepository>();
        _mockEnvironmentRepository = new Mock<IEnvironmentRepository>();
        _mockProjectRepository = new Mock<IProjectRepository>();

        _flagService = new FlagService(
            _mockCreateValidator.Object,
            _mockFlagRepository.Object,
            _mockEnvironmentRepository.Object,
            _mockProjectRepository.Object,
            _mockUpdateValidator.Object);
    }

    [Test]
    public async Task CreateAsync_WithValidData_ShouldCreateFlag()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            Description = "Test flag description",
            Enabled = true,
            ReturnValueTypeString = "Boolean",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<CreateRuleSetDto>()
        };

        var validationResult = new ValidationResult();
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        var environment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Dev,
            ProjectId = 1
        };

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(createDto.EnvironmentId))
            .ReturnsAsync(environment);

        var project = new Project
        {
            Id = 1,
            OrganizationId = 1,
            Name = "Test Project",
            Slug = "test-project"
        };

        _mockProjectRepository.Setup(r => r.GetByIdAsync(createDto.ProjectId))
            .ReturnsAsync(project);

        _mockFlagRepository.Setup(r => r.ExistsAsync(createDto.Key, createDto.EnvironmentId, createDto.ProjectId))
            .ReturnsAsync(false);

        var createdFlag = new Flag
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            Description = "Test flag description",
            Enabled = true,
            ReturnValueType = ReturnValueType.Boolean,
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<RuleSet>()
        };

        _mockFlagRepository.Setup(r => r.CreateAsync(It.IsAny<Flag>()))
            .ReturnsAsync(createdFlag);

        // Act
        var result = await _flagService.CreateAsync(createDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProjectId, Is.EqualTo(1));
        Assert.That(result.EnvironmentId, Is.EqualTo(1));
        Assert.That(result.Key, Is.EqualTo("test-flag"));
        Assert.That(result.Description, Is.EqualTo("Test flag description"));
        Assert.That(result.Enabled, Is.True);
        Assert.That(result.ReturnValueType, Is.EqualTo(ReturnValueType.Boolean));

        _mockEnvironmentRepository.Verify(r => r.GetByIdAsync(createDto.EnvironmentId), Times.Once);
        _mockProjectRepository.Verify(r => r.GetByIdAsync(createDto.ProjectId), Times.Once);
        _mockFlagRepository.Verify(r => r.ExistsAsync(createDto.Key, createDto.EnvironmentId, createDto.ProjectId), Times.Once);
        _mockFlagRepository.Verify(r => r.CreateAsync(It.IsAny<Flag>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "invalid-flag",
            Description = "Invalid flag description",
            Enabled = true,
            ReturnValueTypeString = "Invalid",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<CreateRuleSetDto>()
        };

        var validationResult = new ValidationResult([new ValidationFailure("Key", "Key is required")]);
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _flagService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("Key"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Key is required"));

        _mockEnvironmentRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockProjectRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockFlagRepository.Verify(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockFlagRepository.Verify(r => r.CreateAsync(It.IsAny<Flag>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_WithNonExistentEnvironment_ShouldThrowApplicationException()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 999,
            Key = "test-flag",
            Description = "Test flag description",
            Enabled = true,
            ReturnValueTypeString = "Boolean",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<CreateRuleSetDto>()
        };

        var validationResult = new ValidationResult();
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(createDto.EnvironmentId))
            .ReturnsAsync((Environment?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _flagService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Environment with ID 999 not found."));

        _mockProjectRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockFlagRepository.Verify(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockFlagRepository.Verify(r => r.CreateAsync(It.IsAny<Flag>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_WithNonExistentProject_ShouldThrowApplicationException()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 999,
            EnvironmentId = 1,
            Key = "test-flag",
            Description = "Test flag description",
            Enabled = true,
            ReturnValueTypeString = "Boolean",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<CreateRuleSetDto>()
        };

        var validationResult = new ValidationResult();
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        var environment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Dev,
            ProjectId = 1
        };

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(createDto.EnvironmentId))
            .ReturnsAsync(environment);

        _mockProjectRepository.Setup(r => r.GetByIdAsync(createDto.ProjectId))
            .ReturnsAsync((Project?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _flagService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Project with ID 999 not found."));

        _mockFlagRepository.Verify(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockFlagRepository.Verify(r => r.CreateAsync(It.IsAny<Flag>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_WithExistingKey_ShouldThrowApplicationException()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "existing-flag",
            Description = "Test flag description",
            Enabled = true,
            ReturnValueTypeString = "Boolean",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<CreateRuleSetDto>()
        };

        var validationResult = new ValidationResult();
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        var environment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Dev,
            ProjectId = 1
        };

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(createDto.EnvironmentId))
            .ReturnsAsync(environment);

        var project = new Project
        {
            Id = 1,
            OrganizationId = 1,
            Name = "Test Project",
            Slug = "test-project"
        };

        _mockProjectRepository.Setup(r => r.GetByIdAsync(createDto.ProjectId))
            .ReturnsAsync(project);

        _mockFlagRepository.Setup(r => r.ExistsAsync(createDto.Key, createDto.EnvironmentId, createDto.ProjectId))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _flagService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Flag with key 'existing-flag' already exists in the environment."));

        _mockFlagRepository.Verify(r => r.CreateAsync(It.IsAny<Flag>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithValidData_ShouldUpdateFlag()
    {
        // Arrange
        var updateDto = new UpdateFlagDto
        {
            Id = 1,
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "updated-flag",
            Description = "Updated flag description",
            Enabled = false,
            ReturnValueTypeString = "String",
            DefaultValueOnRaw = "updated-on",
            DefaultValueOffRaw = "updated-off",
            RuleSets = new List<UpdateRuleSetDto>()
        };

        var validationResult = new ValidationResult();
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        var existingFlag = new Flag
        {
            Id = 1,
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "original-flag",
            Description = "Original description",
            Enabled = true,
            ReturnValueType = ReturnValueType.Boolean,
            DefaultValueOnRaw = "original-on",
            DefaultValueOffRaw = "original-off",
            RuleSets = new List<RuleSet>(),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        _mockFlagRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingFlag);

        _mockFlagRepository.Setup(r => r.UpdateAsync(It.IsAny<Flag>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _flagService.UpdateAsync(updateDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Description, Is.EqualTo("Updated flag description"));
        Assert.That(result.Enabled, Is.False);
        Assert.That(result.DefaultValueOnRaw, Is.EqualTo("updated-on"));
        Assert.That(result.DefaultValueOffRaw, Is.EqualTo("updated-off"));

        _mockFlagRepository.Verify(r => r.GetByIdAsync(updateDto.Id), Times.Once);
        _mockFlagRepository.Verify(r => r.UpdateAsync(It.IsAny<Flag>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var updateDto = new UpdateFlagDto
        {
            Id = 1,
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "invalid-flag",
            Description = "Invalid flag description",
            Enabled = true,
            ReturnValueTypeString = "Invalid",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<UpdateRuleSetDto>()
        };

        var validationResult = new ValidationResult([new ValidationFailure("Key", "Key is required")]);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _flagService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("Key"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Key is required"));

        _mockFlagRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockFlagRepository.Verify(r => r.UpdateAsync(It.IsAny<Flag>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowApplicationException()
    {
        // Arrange
        var updateDto = new UpdateFlagDto
        {
            Id = 999,
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "non-existent-flag",
            Description = "Non-existent flag description",
            Enabled = true,
            ReturnValueTypeString = "Boolean",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<UpdateRuleSetDto>()
        };

        var validationResult = new ValidationResult();
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        _mockFlagRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync((Flag?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _flagService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Flag with ID 999 not found."));

        _mockFlagRepository.Verify(r => r.UpdateAsync(It.IsAny<Flag>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithRuleSets_ShouldReconcileRuleSets()
    {
        // Arrange
        var updateDto = new UpdateFlagDto
        {
            Id = 1,
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "flag-with-rules",
            Description = "Flag with rule sets",
            Enabled = true,
            ReturnValueTypeString = "Boolean",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<UpdateRuleSetDto>
            {
                new UpdateRuleSetDto
                {
                    Id = 1,
                    ReturnValueRaw = "enabled",
                    OffReturnValueRaw = "disabled",
                    Priority = 1,
                    Percentage = 100,
                    Conditions = new List<UpdateRuleConditionDto>()
                }
            }
        };

        var validationResult = new ValidationResult();
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        var existingFlag = new Flag
        {
            Id = 1,
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "flag-with-rules",
            Description = "Flag with rule sets",
            Enabled = true,
            ReturnValueType = ReturnValueType.Boolean,
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<RuleSet>
            {
                new RuleSet
                {
                    Id = 1,
                    ReturnValueRaw = "old-enabled",
                    OffReturnValueRaw = "old-disabled",
                    Priority = 1,
                    Percentage = 100,
                    Conditions = new List<RuleCondition>()
                }
            },
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        _mockFlagRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingFlag);

        _mockFlagRepository.Setup(r => r.UpdateAsync(It.IsAny<Flag>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _flagService.UpdateAsync(updateDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));

        _mockFlagRepository.Verify(r => r.GetByIdAsync(updateDto.Id), Times.Once);
        _mockFlagRepository.Verify(r => r.UpdateAsync(It.IsAny<Flag>()), Times.Once);
    }
}

using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Mapping;

public class FlagMappingTests
{
    [Test]
    public void ToDto_Flag_ShouldMapCorrectly()
    {
        // Arrange
        var flag = new Flag
        {
            Id = 1,
            ProjectId = 100,
            EnvironmentId = 200,
            Key = "test-flag",
            Description = "Test flag description",
            Enabled = true,
            UpdatedAt = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero),
            ReturnValueType = ReturnValueType.Boolean,
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<RuleSet>()
        };

        // Act
        var result = flag.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.ProjectId, Is.EqualTo(100));
        Assert.That(result.EnvironmentId, Is.EqualTo(200));
        Assert.That(result.Key, Is.EqualTo("test-flag"));
        Assert.That(result.Description, Is.EqualTo("Test flag description"));
        Assert.That(result.Enabled, Is.True);
        Assert.That(result.UpdatedAt, Is.EqualTo(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero)));
        Assert.That(result.ReturnValueType, Is.EqualTo(ReturnValueType.Boolean));
        Assert.That(result.DefaultValueOnRaw, Is.EqualTo("true"));
        Assert.That(result.DefaultValueOffRaw, Is.EqualTo("false"));
        Assert.That(result.RuleSets, Is.Not.Null);
        Assert.That(result.RuleSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void ToDto_Flag_WithRuleSets_ShouldMapCorrectly()
    {
        // Arrange
        var ruleSet = new RuleSet
        {
            Id = 10,
            ReturnValueRaw = "enabled",
            OffReturnValueRaw = "disabled",
            Priority = 1,
            Percentage = 100,
            Conditions = new List<RuleCondition>()
        };

        var flag = new Flag
        {
            Id = 2,
            ProjectId = 101,
            EnvironmentId = 201,
            Key = "feature-flag",
            Description = "Feature flag with rules",
            Enabled = true,
            UpdatedAt = new DateTimeOffset(2024, 1, 2, 12, 0, 0, TimeSpan.Zero),
            ReturnValueType = ReturnValueType.String,
            DefaultValueOnRaw = "default-on",
            DefaultValueOffRaw = "default-off",
            RuleSets = new List<RuleSet> { ruleSet }
        };

        // Act
        var result = flag.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.Key, Is.EqualTo("feature-flag"));
        Assert.That(result.ReturnValueType, Is.EqualTo(ReturnValueType.String));
        Assert.That(result.RuleSets, Is.Not.Null);
        Assert.That(result.RuleSets.Count, Is.EqualTo(1));
        Assert.That(result.RuleSets[0].Id, Is.EqualTo(10));
        Assert.That(result.RuleSets[0].ReturnValueRaw, Is.EqualTo("enabled"));
        Assert.That(result.RuleSets[0].OffReturnValueRaw, Is.EqualTo("disabled"));
        Assert.That(result.RuleSets[0].Priority, Is.EqualTo(1));
        Assert.That(result.RuleSets[0].Percentage, Is.EqualTo(100));
    }

    [Test]
    public void ToEntity_CreateFlagDto_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 300,
            EnvironmentId = 400,
            Key = "new-flag",
            Description = "New flag description",
            Enabled = false,
            ReturnValueTypeString = "Number",
            DefaultValueOnRaw = "42",
            DefaultValueOffRaw = "0",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProjectId, Is.EqualTo(300));
        Assert.That(result.EnvironmentId, Is.EqualTo(400));
        Assert.That(result.Key, Is.EqualTo("new-flag"));
        Assert.That(result.Description, Is.EqualTo("New flag description"));
        Assert.That(result.Enabled, Is.False);
        Assert.That(result.ReturnValueType, Is.EqualTo(ReturnValueType.Number));
        Assert.That(result.DefaultValueOnRaw, Is.EqualTo("42"));
        Assert.That(result.DefaultValueOffRaw, Is.EqualTo("0"));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
        Assert.That(result.RuleSets, Is.Not.Null);
        Assert.That(result.RuleSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void ToEntity_CreateFlagDto_WithEmptyReturnValueType_ShouldUseDefaultBoolean()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 301,
            EnvironmentId = 401,
            Key = "default-flag",
            Description = "Flag with default return type",
            Enabled = true,
            ReturnValueTypeString = "",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ReturnValueType, Is.EqualTo(ReturnValueType.Boolean)); // Default value
        Assert.That(result.Key, Is.EqualTo("default-flag"));
    }

    [Test]
    public void ToEntity_CreateFlagDto_WithExistingFlag_ShouldUpdateCorrectly()
    {
        // Arrange
        var createDto = new CreateFlagDto
        {
            ProjectId = 302,
            EnvironmentId = 402,
            Key = "updated-flag",
            Description = "Updated flag description",
            Enabled = true,
            ReturnValueTypeString = "Json",
            DefaultValueOnRaw = "{\"enabled\": true}",
            DefaultValueOffRaw = "{\"enabled\": false}",
            RuleSets = new List<CreateRuleSetDto>()
        };

        var existingFlag = new Flag
        {
            Id = 50,
            ProjectId = 302,
            EnvironmentId = 402,
            Key = "old-key",
            Description = "Old description",
            Enabled = false,
            ReturnValueType = ReturnValueType.Boolean,
            DefaultValueOnRaw = "false",
            DefaultValueOffRaw = "true",
            RuleSets = new List<RuleSet>()
        };

        // Act
        var result = createDto.ToEntity(existingFlag);

        // Assert
        Assert.That(result, Is.SameAs(existingFlag));
        Assert.That(result.Id, Is.EqualTo(50)); // Should preserve existing ID
        Assert.That(result.Key, Is.EqualTo("updated-flag"));
        Assert.That(result.Description, Is.EqualTo("Updated flag description"));
        Assert.That(result.Enabled, Is.True);
        Assert.That(result.ReturnValueType, Is.EqualTo(ReturnValueType.Json));
        Assert.That(result.DefaultValueOnRaw, Is.EqualTo("{\"enabled\": true}"));
        Assert.That(result.DefaultValueOffRaw, Is.EqualTo("{\"enabled\": false}"));
    }

    [Test]
    public void UpdateEntity_UpdateFlagDto_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateFlagDto
        {
            Id = 60,
            ProjectId = 303,
            EnvironmentId = 403,
            Key = "update-flag",
            Description = "Updated description",
            Enabled = false,
            ReturnValueTypeString = "String",
            DefaultValueOnRaw = "updated-on",
            DefaultValueOffRaw = "updated-off",
            RuleSets = new List<UpdateRuleSetDto>()
        };

        var existingFlag = new Flag
        {
            Id = 60,
            ProjectId = 303,
            EnvironmentId = 403,
            Key = "original-key",
            Description = "Original description",
            Enabled = true,
            ReturnValueType = ReturnValueType.Boolean,
            DefaultValueOnRaw = "original-on",
            DefaultValueOffRaw = "original-off",
            RuleSets = new List<RuleSet>()
        };

        // Act
        updateDto.UpdateEntity(existingFlag);

        // Assert
        Assert.That(existingFlag.Description, Is.EqualTo("Updated description"));
        Assert.That(existingFlag.Enabled, Is.False);
        Assert.That(existingFlag.DefaultValueOnRaw, Is.EqualTo("updated-on"));
        Assert.That(existingFlag.DefaultValueOffRaw, Is.EqualTo("updated-off"));
        // These should NOT be updated by UpdateEntity method
        Assert.That(existingFlag.Key, Is.EqualTo("original-key"));
        Assert.That(existingFlag.ReturnValueType, Is.EqualTo(ReturnValueType.Boolean));
    }
}

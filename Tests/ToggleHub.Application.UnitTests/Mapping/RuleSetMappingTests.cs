using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Mapping;

public class RuleSetMappingTests
{
    [Test]
    public void ToDto_RuleSet_ShouldMapCorrectly()
    {
        // Arrange
        var ruleSet = new RuleSet
        {
            Id = 1,
            ReturnValueRaw = "enabled",
            OffReturnValueRaw = "disabled",
            Priority = 1,
            Percentage = 100,
            Conditions = new List<RuleCondition>()
        };

        // Act
        var result = ruleSet.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.ReturnValueRaw, Is.EqualTo("enabled"));
        Assert.That(result.OffReturnValueRaw, Is.EqualTo("disabled"));
        Assert.That(result.Priority, Is.EqualTo(1));
        Assert.That(result.Percentage, Is.EqualTo(100));
        Assert.That(result.Conditions, Is.Not.Null);
        Assert.That(result.Conditions.Count, Is.EqualTo(0));
    }

    [Test]
    public void ToDto_RuleSet_WithConditions_ShouldMapCorrectly()
    {
        // Arrange
        var condition = new RuleCondition
        {
            Id = 10,
            Field = "userId",
            FieldType = RuleFieldType.Number,
            Operator = OperatorType.Equals,
            ValueNumber = 123,
            Items = new List<RuleConditionItem>()
        };

        var ruleSet = new RuleSet
        {
            Id = 2,
            ReturnValueRaw = "feature-on",
            OffReturnValueRaw = "feature-off",
            Priority = 2,
            Percentage = 50,
            Conditions = new List<RuleCondition> { condition }
        };

        // Act
        var result = ruleSet.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.ReturnValueRaw, Is.EqualTo("feature-on"));
        Assert.That(result.OffReturnValueRaw, Is.EqualTo("feature-off"));
        Assert.That(result.Priority, Is.EqualTo(2));
        Assert.That(result.Percentage, Is.EqualTo(50));
        Assert.That(result.Conditions, Is.Not.Null);
        Assert.That(result.Conditions.Count, Is.EqualTo(1));
        Assert.That(result.Conditions[0].Id, Is.EqualTo(10));
        Assert.That(result.Conditions[0].Field, Is.EqualTo("userId"));
        Assert.That(result.Conditions[0].FieldType, Is.EqualTo(RuleFieldType.Number));
        Assert.That(result.Conditions[0].Operator, Is.EqualTo(OperatorType.Equals));
        Assert.That(result.Conditions[0].ValueNumber, Is.EqualTo(123));
    }

    [Test]
    public void ToEntity_CreateRuleSetDto_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleSetDto
        {
            ReturnValueRaw = "new-enabled",
            OffReturnValueRaw = "new-disabled",
            Priority = 3,
            Percentage = 75,
            Conditions = new List<CreateRuleConditionDto>()
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ReturnValueRaw, Is.EqualTo("new-enabled"));
        Assert.That(result.OffReturnValueRaw, Is.EqualTo("new-disabled"));
        Assert.That(result.Priority, Is.EqualTo(3));
        Assert.That(result.Percentage, Is.EqualTo(75));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
        Assert.That(result.Conditions, Is.Not.Null);
        Assert.That(result.Conditions.Count, Is.EqualTo(0));
    }

    [Test]
    public void ToEntity_CreateRuleSetDto_WithExistingRuleSet_ShouldUpdateCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleSetDto
        {
            ReturnValueRaw = "updated-enabled",
            OffReturnValueRaw = "updated-disabled",
            Priority = 4,
            Percentage = 80,
            Conditions = new List<CreateRuleConditionDto>()
        };

        var existingRuleSet = new RuleSet
        {
            Id = 20,
            ReturnValueRaw = "old-enabled",
            OffReturnValueRaw = "old-disabled",
            Priority = 1,
            Percentage = 100,
            Conditions = new List<RuleCondition>()
        };

        // Act
        var result = createDto.ToEntity(existingRuleSet);

        // Assert
        Assert.That(result, Is.SameAs(existingRuleSet));
        Assert.That(result.Id, Is.EqualTo(20)); // Should preserve existing ID
        Assert.That(result.ReturnValueRaw, Is.EqualTo("updated-enabled"));
        Assert.That(result.OffReturnValueRaw, Is.EqualTo("updated-disabled"));
        Assert.That(result.Priority, Is.EqualTo(4));
        Assert.That(result.Percentage, Is.EqualTo(80));
    }

    [Test]
    public void ToEntity_UpdateRuleSetDto_ShouldMapCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleSetDto
        {
            ReturnValueRaw = "update-enabled",
            OffReturnValueRaw = "update-disabled",
            Priority = 5,
            Percentage = 90
        };

        // Act
        var result = updateDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ReturnValueRaw, Is.EqualTo("update-enabled"));
        Assert.That(result.OffReturnValueRaw, Is.EqualTo("update-disabled"));
        Assert.That(result.Priority, Is.EqualTo(5));
        Assert.That(result.Percentage, Is.EqualTo(90));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_UpdateRuleSetDto_WithExistingRuleSet_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleSetDto
        {
            ReturnValueRaw = "completely-updated-enabled",
            OffReturnValueRaw = "completely-updated-disabled",
            Priority = 6,
            Percentage = 95
        };

        var existingRuleSet = new RuleSet
        {
            Id = 25,
            ReturnValueRaw = "original-enabled",
            OffReturnValueRaw = "original-disabled",
            Priority = 1,
            Percentage = 100,
            Conditions = new List<RuleCondition>()
        };

        // Act
        var result = updateDto.ToEntity(existingRuleSet);

        // Assert
        Assert.That(result, Is.SameAs(existingRuleSet));
        Assert.That(result.Id, Is.EqualTo(25)); // Should preserve existing ID
        Assert.That(result.ReturnValueRaw, Is.EqualTo("completely-updated-enabled"));
        Assert.That(result.OffReturnValueRaw, Is.EqualTo("completely-updated-disabled"));
        Assert.That(result.Priority, Is.EqualTo(6));
        Assert.That(result.Percentage, Is.EqualTo(95));
    }

    [Test]
    public void UpdateEntity_UpdateRuleSetDto_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleSetDto
        {
            ReturnValueRaw = "patch-enabled",
            OffReturnValueRaw = "patch-disabled",
            Priority = 7,
            Percentage = 85
        };

        var existingRuleSet = new RuleSet
        {
            Id = 30,
            ReturnValueRaw = "original-enabled",
            OffReturnValueRaw = "original-disabled",
            Priority = 1,
            Percentage = 100,
            Conditions = new List<RuleCondition>()
        };

        // Act
        updateDto.UpdateEntity(existingRuleSet);

        // Assert
        Assert.That(existingRuleSet.ReturnValueRaw, Is.EqualTo("patch-enabled"));
        Assert.That(existingRuleSet.OffReturnValueRaw, Is.EqualTo("patch-disabled"));
        Assert.That(existingRuleSet.Priority, Is.EqualTo(7));
        Assert.That(existingRuleSet.Percentage, Is.EqualTo(85));
        // These should NOT be updated by UpdateEntity method
        Assert.That(existingRuleSet.Id, Is.EqualTo(30)); // Should preserve existing ID
    }
}

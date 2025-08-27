using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Mapping;

public class RuleConditionMappingTests
{
    [Test]
    public void ToDto_RuleCondition_ShouldMapCorrectly()
    {
        // Arrange
        var condition = new RuleCondition
        {
            Id = 1,
            Field = "userId",
            FieldType = RuleFieldType.Number,
            Operator = OperatorType.Equals,
            ValueString = "123",
            ValueNumber = 123,
            ValueBoolean = null,
            Items = new List<RuleConditionItem>()
        };

        // Act
        var result = condition.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Field, Is.EqualTo("userId"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.Number));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.Equals));
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueNumber, Is.EqualTo(123));
        Assert.That(result.ValueBoolean, Is.Null);
        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(0));
    }

    [Test]
    public void ToDto_RuleCondition_WithBooleanValues_ShouldMapCorrectly()
    {
        // Arrange
        var condition = new RuleCondition
        {
            Id = 2,
            Field = "isPremium",
            FieldType = RuleFieldType.Boolean,
            Operator = OperatorType.Equals,
            ValueString = null,
            ValueNumber = null,
            ValueBoolean = true,
            Items = new List<RuleConditionItem>()
        };

        // Act
        var result = condition.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.Field, Is.EqualTo("isPremium"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.Boolean));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.Equals));
        Assert.That(result.ValueString, Is.Null);
        Assert.That(result.ValueNumber, Is.Null);
        Assert.That(result.ValueBoolean, Is.True);
    }

    [Test]
    public void ToDto_RuleCondition_WithItems_ShouldMapCorrectly()
    {
        // Arrange
        var item1 = new RuleConditionItem
        {
            Id = 10,
            ValueString = "item1",
            ValueNumber = 10
        };

        var item2 = new RuleConditionItem
        {
            Id = 11,
            ValueString = "item2",
            ValueNumber = 20
        };

        var condition = new RuleCondition
        {
            Id = 3,
            Field = "userRole",
            FieldType = RuleFieldType.List,
            Operator = OperatorType.In,
            ValueString = null,
            ValueNumber = null,
            ValueBoolean = null,
            Items = new List<RuleConditionItem> { item1, item2 }
        };

        // Act
        var result = condition.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.Field, Is.EqualTo("userRole"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.List));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.In));
        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.Items[0].Id, Is.EqualTo(10));
        Assert.That(result.Items[0].ValueString, Is.EqualTo("item1"));
        Assert.That(result.Items[0].ValueNumber, Is.EqualTo(10));
        Assert.That(result.Items[1].Id, Is.EqualTo(11));
        Assert.That(result.Items[1].ValueString, Is.EqualTo("item2"));
        Assert.That(result.Items[1].ValueNumber, Is.EqualTo(20));
    }

    [Test]
    public void ToEntity_CreateRuleConditionDto_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleConditionDto
        {
            Field = "newField",
            FieldTypeString = "String",
            OperatorString = "Contains",
            ValueString = "test value",
            ValueNumber = null,
            ValueBoolean = null,
            Items = new List<CreateRuleConditionItemDto>()
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Field, Is.EqualTo("newField"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.String));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.Contains));
        Assert.That(result.ValueString, Is.EqualTo("test value"));
        Assert.That(result.ValueNumber, Is.Null);
        Assert.That(result.ValueBoolean, Is.Null);
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(0));
    }

    [Test]
    public void ToEntity_CreateRuleConditionDto_WithEmptyFieldType_ShouldUseDefaultString()
    {
        // Arrange
        var createDto = new CreateRuleConditionDto
        {
            Field = "defaultField",
            FieldTypeString = "",
            OperatorString = "Equals",
            ValueString = "default value",
            ValueNumber = null,
            ValueBoolean = null,
            Items = new List<CreateRuleConditionItemDto>()
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Field, Is.EqualTo("defaultField"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.String)); // Default value
        Assert.That(result.Operator, Is.EqualTo(OperatorType.Equals));
    }

    [Test]
    public void ToEntity_CreateRuleConditionDto_WithEmptyOperator_ShouldUseDefaultEquals()
    {
        // Arrange
        var createDto = new CreateRuleConditionDto
        {
            Field = "operatorField",
            FieldTypeString = "Number",
            OperatorString = "",
            ValueString = null,
            ValueNumber = 42,
            ValueBoolean = null,
            Items = new List<CreateRuleConditionItemDto>()
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Field, Is.EqualTo("operatorField"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.Number));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.Equals)); // Default value
        Assert.That(result.ValueNumber, Is.EqualTo(42));
    }

    [Test]
    public void ToEntity_CreateRuleConditionDto_WithExistingCondition_ShouldUpdateCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleConditionDto
        {
            Field = "updatedField",
            FieldTypeString = "Boolean",
            OperatorString = "NotEquals",
            ValueString = null,
            ValueNumber = null,
            ValueBoolean = false,
            Items = new List<CreateRuleConditionItemDto>()
        };

        var existingCondition = new RuleCondition
        {
            Id = 20,
            Field = "oldField",
            FieldType = RuleFieldType.String,
            Operator = OperatorType.Equals,
            ValueString = "old value",
            ValueNumber = null,
            ValueBoolean = null,
            Items = new List<RuleConditionItem>()
        };

        // Act
        var result = createDto.ToEntity(existingCondition);

        // Assert
        Assert.That(result, Is.SameAs(existingCondition));
        Assert.That(result.Id, Is.EqualTo(20)); // Should preserve existing ID
        Assert.That(result.Field, Is.EqualTo("updatedField"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.Boolean));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.NotEquals));
        Assert.That(result.ValueString, Is.Null);
        Assert.That(result.ValueBoolean, Is.False);
    }

    [Test]
    public void ToEntity_UpdateRuleConditionDto_ShouldMapCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionDto
        {
            Field = "updateField",
            FieldTypeString = "List",
            OperatorString = "In",
            ValueString = "list value",
            ValueNumber = null,
            ValueBoolean = null
        };

        // Act
        var result = updateDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Field, Is.EqualTo("updateField"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.List));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.In));
        Assert.That(result.ValueString, Is.EqualTo("list value"));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_UpdateRuleConditionDto_WithExistingCondition_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionDto
        {
            Field = "completelyUpdatedField",
            FieldTypeString = "Number",
            OperatorString = "GreaterThan",
            ValueString = null,
            ValueNumber = 100,
            ValueBoolean = null
        };

        var existingCondition = new RuleCondition
        {
            Id = 25,
            Field = "originalField",
            FieldType = RuleFieldType.String,
            Operator = OperatorType.Equals,
            ValueString = "original value",
            ValueNumber = null,
            ValueBoolean = null,
            Items = new List<RuleConditionItem>()
        };

        // Act
        var result = updateDto.ToEntity(existingCondition);

        // Assert
        Assert.That(result, Is.SameAs(existingCondition));
        Assert.That(result.Id, Is.EqualTo(25)); // Should preserve existing ID
        Assert.That(result.Field, Is.EqualTo("completelyUpdatedField"));
        Assert.That(result.FieldType, Is.EqualTo(RuleFieldType.Number));
        Assert.That(result.Operator, Is.EqualTo(OperatorType.GreaterThan));
        Assert.That(result.ValueNumber, Is.EqualTo(100));
    }

    [Test]
    public void UpdateEntity_UpdateRuleConditionDto_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionDto
        {
            Field = "patchField",
            FieldTypeString = "String",
            OperatorString = "StartsWith",
            ValueString = "patch value",
            ValueNumber = null,
            ValueBoolean = null
        };

        var existingCondition = new RuleCondition
        {
            Id = 30,
            Field = "originalField",
            FieldType = RuleFieldType.String,
            Operator = OperatorType.Equals,
            ValueString = "original value",
            ValueNumber = null,
            ValueBoolean = null,
            Items = new List<RuleConditionItem>()
        };

        // Act
        updateDto.UpdateEntity(existingCondition);

        // Assert
        Assert.That(existingCondition.Field, Is.EqualTo("patchField"));
        Assert.That(existingCondition.FieldType, Is.EqualTo(RuleFieldType.String));
        Assert.That(existingCondition.Operator, Is.EqualTo(OperatorType.StartsWith));
        Assert.That(existingCondition.ValueString, Is.EqualTo("patch value"));
        // These should NOT be updated by UpdateEntity method
        Assert.That(existingCondition.Id, Is.EqualTo(30)); // Should preserve existing ID
    }

    [Test]
    public void UpdateEntity_UpdateRuleConditionDto_WithEmptyFieldType_ShouldPreserveExistingFieldType()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionDto
        {
            Field = "preserveField",
            FieldTypeString = "",
            OperatorString = "EndsWith",
            ValueString = "preserve value",
            ValueNumber = null,
            ValueBoolean = null
        };

        var existingCondition = new RuleCondition
        {
            Id = 35,
            Field = "originalField",
            FieldType = RuleFieldType.Number,
            Operator = OperatorType.Equals,
            ValueString = "original value",
            ValueNumber = null,
            ValueBoolean = null,
            Items = new List<RuleConditionItem>()
        };

        // Act
        updateDto.UpdateEntity(existingCondition);

        // Assert
        Assert.That(existingCondition.Field, Is.EqualTo("preserveField"));
        Assert.That(existingCondition.FieldType, Is.EqualTo(RuleFieldType.Number)); // Should preserve existing value
        Assert.That(existingCondition.Operator, Is.EqualTo(OperatorType.EndsWith));
        Assert.That(existingCondition.ValueString, Is.EqualTo("preserve value"));
    }
}

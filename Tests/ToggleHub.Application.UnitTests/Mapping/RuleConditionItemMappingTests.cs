using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Mapping;

public class RuleConditionItemMappingTests
{
    [Test]
    public void ToDto_RuleConditionItem_ShouldMapCorrectly()
    {
        // Arrange
        var item = new RuleConditionItem
        {
            Id = 1,
            ValueString = "test value",
            ValueNumber = 42
        };

        // Act
        var result = item.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.ValueString, Is.EqualTo("test value"));
        Assert.That(result.ValueNumber, Is.EqualTo(42));
    }

    [Test]
    public void ToDto_RuleConditionItem_WithStringOnly_ShouldMapCorrectly()
    {
        // Arrange
        var item = new RuleConditionItem
        {
            Id = 2,
            ValueString = "string only value",
            ValueNumber = null
        };

        // Act
        var result = item.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.ValueString, Is.EqualTo("string only value"));
        Assert.That(result.ValueNumber, Is.Null);
    }

    [Test]
    public void ToDto_RuleConditionItem_WithNumberOnly_ShouldMapCorrectly()
    {
        // Arrange
        var item = new RuleConditionItem
        {
            Id = 3,
            ValueString = null,
            ValueNumber = 3.14m
        };

        // Act
        var result = item.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.ValueString, Is.Null);
        Assert.That(result.ValueNumber, Is.EqualTo(3.14m));
    }

    [Test]
    public void ToDto_RuleConditionItem_WithNullValues_ShouldMapCorrectly()
    {
        // Arrange
        var item = new RuleConditionItem
        {
            Id = 4,
            ValueString = null,
            ValueNumber = null
        };

        // Act
        var result = item.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(4));
        Assert.That(result.ValueString, Is.Null);
        Assert.That(result.ValueNumber, Is.Null);
    }

    [Test]
    public void ToEntity_CreateRuleConditionItemDto_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleConditionItemDto
        {
            ValueString = "new string value",
            ValueNumber = 100
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ValueString, Is.EqualTo("new string value"));
        Assert.That(result.ValueNumber, Is.EqualTo(100));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_CreateRuleConditionItemDto_WithStringOnly_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleConditionItemDto
        {
            ValueString = "string only",
            ValueNumber = null
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ValueString, Is.EqualTo("string only"));
        Assert.That(result.ValueNumber, Is.Null);
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_CreateRuleConditionItemDto_WithNumberOnly_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleConditionItemDto
        {
            ValueString = null,
            ValueNumber = 999.99m
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ValueString, Is.Null);
        Assert.That(result.ValueNumber, Is.EqualTo(999.99m));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_CreateRuleConditionItemDto_WithExistingItem_ShouldUpdateCorrectly()
    {
        // Arrange
        var createDto = new CreateRuleConditionItemDto
        {
            ValueString = "updated string",
            ValueNumber = 200
        };

        var existingItem = new RuleConditionItem
        {
            Id = 20,
            ValueString = "old string",
            ValueNumber = 50
        };

        // Act
        var result = createDto.ToEntity(existingItem);

        // Assert
        Assert.That(result, Is.SameAs(existingItem));
        Assert.That(result.Id, Is.EqualTo(20)); // Should preserve existing ID
        Assert.That(result.ValueString, Is.EqualTo("updated string"));
        Assert.That(result.ValueNumber, Is.EqualTo(200));
    }

    [Test]
    public void ToEntity_UpdateRuleConditionItemDto_ShouldMapCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionItemDto
        {
            ValueString = "update string",
            ValueNumber = 300
        };

        // Act
        var result = updateDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ValueString, Is.EqualTo("update string"));
        Assert.That(result.ValueNumber, Is.EqualTo(300));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_UpdateRuleConditionItemDto_WithExistingItem_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionItemDto
        {
            ValueString = "completely updated string",
            ValueNumber = 400
        };

        var existingItem = new RuleConditionItem
        {
            Id = 25,
            ValueString = "original string",
            ValueNumber = 75
        };

        // Act
        var result = updateDto.ToEntity(existingItem);

        // Assert
        Assert.That(result, Is.SameAs(existingItem));
        Assert.That(result.Id, Is.EqualTo(25)); // Should preserve existing ID
        Assert.That(result.ValueString, Is.EqualTo("completely updated string"));
        Assert.That(result.ValueNumber, Is.EqualTo(400));
    }

    [Test]
    public void UpdateEntity_UpdateRuleConditionItemDto_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionItemDto
        {
            ValueString = "patch string",
            ValueNumber = 500
        };

        var existingItem = new RuleConditionItem
        {
            Id = 30,
            ValueString = "original string",
            ValueNumber = 100
        };

        // Act
        updateDto.UpdateEntity(existingItem);

        // Assert
        Assert.That(existingItem.ValueString, Is.EqualTo("patch string"));
        Assert.That(existingItem.ValueNumber, Is.EqualTo(500));
        // This should NOT be updated by UpdateEntity method
        Assert.That(existingItem.Id, Is.EqualTo(30)); // Should preserve existing ID
    }

    [Test]
    public void UpdateEntity_UpdateRuleConditionItemDto_WithNullValues_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateRuleConditionItemDto
        {
            ValueString = null,
            ValueNumber = null
        };

        var existingItem = new RuleConditionItem
        {
            Id = 35,
            ValueString = "original string",
            ValueNumber = 150
        };

        // Act
        updateDto.UpdateEntity(existingItem);

        // Assert
        Assert.That(existingItem.ValueString, Is.Null);
        Assert.That(existingItem.ValueNumber, Is.Null);
        Assert.That(existingItem.Id, Is.EqualTo(35)); // Should preserve existing ID
    }
}

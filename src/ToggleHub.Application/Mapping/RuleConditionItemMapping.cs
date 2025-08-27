using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class RuleConditionItemMapping
{
    public static RuleConditionItemDto ToDto(this RuleConditionItem item)
    {
        return new RuleConditionItemDto
        {
            Id = item.Id,
            ValueString = item.ValueString,
            ValueNumber = item.ValueNumber
        };
    }

    public static RuleConditionItem ToEntity(this CreateRuleConditionItemDto createDto, RuleConditionItem? item = null)
    {
        item ??= new RuleConditionItem();
        item.ValueString = createDto.ValueString;
        item.ValueNumber = createDto.ValueNumber;
        return item;
    }
    
    public static RuleConditionItem ToEntity(this UpdateRuleConditionItemDto updateDto, RuleConditionItem? item = null)
    {
        item ??= new RuleConditionItem();
        item.ValueString = updateDto.ValueString;
        item.ValueNumber = updateDto.ValueNumber;
        return item;
    }
    
    public static void UpdateEntity(this UpdateRuleConditionItemDto updateDto, RuleConditionItem item)
    {
        item.ValueString = updateDto.ValueString;
        item.ValueNumber = updateDto.ValueNumber;
    }
}

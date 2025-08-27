using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class RuleConditionMapping
{
    public static RuleConditionDto ToDto(this RuleCondition condition)
    {
        return new RuleConditionDto
        {
            Id = condition.Id,
            Field = condition.Field,
            FieldType = condition.FieldType,
            Operator = condition.Operator,
            ValueString = condition.ValueString,
            ValueNumber = condition.ValueNumber,
            ValueBoolean = condition.ValueBoolean,
            Items = condition.Items.Select(i => i.ToDto()).ToList()
        };
    }

    public static RuleCondition ToEntity(this CreateRuleConditionDto createDto, RuleCondition? condition = null)
    {
        condition ??= new RuleCondition();
        condition.Field = createDto.Field;
        condition.FieldType = createDto.FieldType ?? RuleFieldType.String;
        condition.Operator = createDto.Operator ?? OperatorType.Equals;
        condition.ValueString = createDto.ValueString;
        condition.ValueNumber = createDto.ValueNumber;
        condition.ValueBoolean = createDto.ValueBoolean;
        condition.Items = createDto.Items.Select(i => i.ToEntity()).ToList();
        return condition;
    }
    
    public static RuleCondition ToEntity(this UpdateRuleConditionDto updateDto, RuleCondition? condition = null)
    {
        condition ??= new RuleCondition();
        condition.Field = updateDto.Field;
        condition.FieldType = updateDto.FieldType ?? RuleFieldType.String;
        condition.Operator = updateDto.Operator ?? OperatorType.Equals;
        condition.ValueString = updateDto.ValueString;
        condition.ValueNumber = updateDto.ValueNumber;
        condition.ValueBoolean = updateDto.ValueBoolean;
        return condition;
    }
    
    public static void UpdateEntity(this UpdateRuleConditionDto updateDto, RuleCondition condition)
    {
        condition.Field = updateDto.Field;
        if (updateDto.FieldType.HasValue)
            condition.FieldType = updateDto.FieldType.Value;
        if (updateDto.Operator.HasValue)
            condition.Operator = updateDto.Operator.Value;
        condition.ValueString = updateDto.ValueString;
        condition.ValueNumber = updateDto.ValueNumber;
        condition.ValueBoolean = updateDto.ValueBoolean;
    }
}

using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class RuleSetMapping
{
    public static RuleSetDto ToDto(this RuleSet ruleSet)
    {
        return new RuleSetDto
        {
            Id = ruleSet.Id,
            ReturnValueRaw = ruleSet.ReturnValueRaw,
            OffReturnValueRaw = ruleSet.OffReturnValueRaw,
            Priority = ruleSet.Priority,
            Percentage = ruleSet.Percentage,
            Conditions = ruleSet.Conditions.Select(c => c.ToDto()).ToList()
        };
    }

    public static RuleSet ToEntity(this CreateRuleSetDto createDto, RuleSet? ruleSet = null)
    {
        ruleSet ??= new RuleSet();
        ruleSet.ReturnValueRaw = createDto.ReturnValueRaw;
        ruleSet.OffReturnValueRaw = createDto.OffReturnValueRaw;
        ruleSet.Priority = createDto.Priority;
        ruleSet.Percentage = createDto.Percentage;
        ruleSet.Conditions = createDto.Conditions.Select(c => c.ToEntity()).ToList();
        return ruleSet;
    }
    
    public static RuleSet ToEntity(this UpdateRuleSetDto updateDto, RuleSet? ruleSet = null)
    {
        ruleSet ??= new RuleSet();
        ruleSet.ReturnValueRaw = updateDto.ReturnValueRaw;
        ruleSet.OffReturnValueRaw = updateDto.OffReturnValueRaw;
        ruleSet.Priority = updateDto.Priority;
        ruleSet.Percentage = updateDto.Percentage;
        return ruleSet;
    }
    
    public static void UpdateEntity(this UpdateRuleSetDto updateDto, RuleSet ruleSet)
    {
        ruleSet.ReturnValueRaw = updateDto.ReturnValueRaw;
        ruleSet.OffReturnValueRaw = updateDto.OffReturnValueRaw;
        ruleSet.Priority = updateDto.Priority;
        ruleSet.Percentage = updateDto.Percentage;
    }
}

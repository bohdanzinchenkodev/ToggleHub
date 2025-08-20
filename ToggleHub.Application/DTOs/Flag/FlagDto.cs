using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag;

public class FlagDto : BaseFlagDto
{
    public int Id { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public IList<RuleSetDto> RuleSets { get; set; } = new List<RuleSetDto>();
}

public class RuleSetDto : BaseRuleSetDto
{
    public int Id { get; set; }
    public IList<RuleConditionDto> Conditions { get; set; } = new List<RuleConditionDto>();
}

public class RuleConditionDto : BaseRuleConditionDto
{
    public int Id { get; set; }
    public IList<RuleConditionItemDto> Items { get; set; } = new List<RuleConditionItemDto>();
}

public class RuleConditionItemDto : BaseRuleConditionItemDto
{
    public int Id { get; set; }
}

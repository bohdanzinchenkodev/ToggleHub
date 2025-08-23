namespace ToggleHub.Domain.Entities;

public class RuleConditionItem : BaseEntity
{
    public int RuleConditionId { get; set; }
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }

    public RuleCondition RuleCondition { get; set; } = null!;
}
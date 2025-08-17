namespace ToggleHub.Domain.Entities;

public class RuleSet : BaseEntity
{
    public int FlagId { get; set; }
    public int Priority { get; set; }                 
    public int Percentage { get; set; } = 100;         
    public ICollection<RuleCondition> Conditions { get; set; } = new List<RuleCondition>();
    public Flag Flag { get; set; } = null!;
}

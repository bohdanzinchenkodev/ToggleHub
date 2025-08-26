namespace ToggleHub.Domain.Entities;

public class RuleSet : BaseEntity
{
    public int FlagId { get; set; }
    public Guid BucketingSeed { get; set; } = Guid.NewGuid();
    public string? ReturnValueRaw { get; set; }
    public string? OffReturnValueRaw { get; set; } 
    public int Priority { get; set; }                 
    public int Percentage { get; set; } = 100;  
    public ICollection<RuleCondition> Conditions { get; set; } = new List<RuleCondition>();
    public Flag Flag { get; set; } = null!;
}
public enum ReturnValueType
{
    Boolean = 10,
    Number = 20,
    String = 30,
    Json = 40
}

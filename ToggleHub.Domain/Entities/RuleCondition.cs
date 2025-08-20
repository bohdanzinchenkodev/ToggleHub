namespace ToggleHub.Domain.Entities;

public class RuleCondition : BaseEntity
{
    public int RuleSetId { get; set; }
    public required string Field { get; set; }        
    public RuleFieldType FieldType { get; set; }
    public OperatorType Operator { get; set; }
    public string? ValueString { get; set; }          
    public decimal? ValueNumber { get; set; }         
    public bool? ValueBoolean { get; set; }
    public ICollection<RuleConditionItem> Items { get; set; } = new List<RuleConditionItem>();
    public RuleSet RuleSet { get; set; } = null!;
}
public enum RuleFieldType
{
    Boolean = 10,
    String = 20,
    Number = 30,
    List = 40,
}
public enum OperatorType
{
    Equals = 10,
    NotEquals = 20,
    GreaterThan = 30,
    LessThan = 40,
    Contains = 50,
    StartsWith = 60,
    EndsWith = 70,
    In = 80,
    NotIn = 90,
}
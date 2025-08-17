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
    Boolean,
    String,
    Number,
    List
}
public enum OperatorType
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    Contains,
    StartsWith,
    EndsWith,
    In,
    NotIn
}
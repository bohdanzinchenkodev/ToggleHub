using ToggleHub.Domain.Attributes;

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
    [ValidRuleFieldTypes(RuleFieldType.Boolean, RuleFieldType.String, RuleFieldType.Number)]
    Equals = 10,
    
    [ValidRuleFieldTypes(RuleFieldType.Boolean, RuleFieldType.String, RuleFieldType.Number)]
    NotEquals = 20,
    
    [ValidRuleFieldTypes(RuleFieldType.Number)]
    GreaterThan = 30,
    
    [ValidRuleFieldTypes(RuleFieldType.Number)]
    LessThan = 40,
    
    [ValidRuleFieldTypes(RuleFieldType.String)]
    Contains = 50,
    
    [ValidRuleFieldTypes(RuleFieldType.String)]
    StartsWith = 60,
    
    [ValidRuleFieldTypes(RuleFieldType.String)]
    EndsWith = 70,
    
    [ValidRuleFieldTypes(RuleFieldType.List)]
    In = 80,
    
    [ValidRuleFieldTypes(RuleFieldType.List)]
    NotIn = 90,
}
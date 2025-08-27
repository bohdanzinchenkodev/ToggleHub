namespace ToggleHub.Domain.Entities;

public class Flag : BaseEntity
{
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Project Project { get; set; } = null!;
    public Environment Environment { get; set; } = null!;
    public string? DefaultValueOnRaw { get; set; }
    public string? DefaultValueOffRaw { get; set; } 
    public ReturnValueType ReturnValueType { get; set; } = ReturnValueType.Boolean;
    public ICollection<RuleSet> RuleSets { get; set; } = new List<RuleSet>();
}
namespace ToggleHub.Domain.Entities;

public class Flag : BaseEntity
{
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public required string Key { get; set; }
    public FlagType Type { get; set; } = FlagType.Boolean;
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public string? DefaultValueRaw { get; set; }
    public Guid BucketingSeed { get; set; } = Guid.NewGuid();
    public DateTimeOffset UpdatedAt { get; set; }
    public Project Project { get; set; } = null!;
    public Environment Environment { get; set; } = null!;

    public ICollection<RuleSet> RuleSets { get; set; } = new List<RuleSet>();
}

//what type of value the flag can hold
public enum FlagType
{
    Boolean,
    String,
    Number,
    JSON
}
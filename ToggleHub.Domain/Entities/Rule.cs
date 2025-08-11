namespace ToggleHub.Domain.Entities;

public class Rule : BaseEntity
{
    public int FlagId { get; set; }
    public int Priority { get; set; }
    public string ConditionsJson { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public Flag Flag { get; set; } = null!;
}

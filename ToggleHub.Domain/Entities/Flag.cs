namespace ToggleHub.Domain.Entities;

public class Flag : BaseEntity
{
    public int OrgId { get; set; }
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Environment Environment { get; set; } = null!;
}

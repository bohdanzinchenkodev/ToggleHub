namespace ToggleHub.Domain.Entities;


public class ApiKey : BaseEntity
{
    public int OrganizationId { get; set; }
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public string Key { get; set; } = string.Empty;
    
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Environment Environment { get; set; } = null!;
}

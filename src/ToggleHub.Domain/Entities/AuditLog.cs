namespace ToggleHub.Domain.Entities;

public class AuditLog : BaseEntity
{
    public int OrganizationId { get; set; }
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public string Actor { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public int TargetId { get; set; }
    public string DiffJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Environment Environment { get; set; } = null!;
}

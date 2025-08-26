namespace ToggleHub.Domain.Entities;

public enum EnvironmentType
{
    Dev = 10,
    Staging = 20,
    Prod = 30
}

public class Environment : BaseEntity
{
    public int OrganizationId { get; set; }
    public EnvironmentType Type { get; set; }
    
    // Navigation property
    public Organization Organization { get; set; } = null!;
}

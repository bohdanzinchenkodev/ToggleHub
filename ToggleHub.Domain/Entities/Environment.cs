namespace ToggleHub.Domain.Entities;

public enum EnvironmentType
{
    Dev,
    Staging,
    Prod
}

public class Environment : BaseEntity
{
    public int ProjectId { get; set; }
    public EnvironmentType Name { get; set; }
    
    // Navigation property
    public Project Project { get; set; } = null!;
}

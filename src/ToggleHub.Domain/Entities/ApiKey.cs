namespace ToggleHub.Domain.Entities;

public enum ApiKeyType
{
    Server,
    Client
}

public class ApiKey : BaseEntity
{
    public int OrganizationId { get; set; }
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public ApiKeyType Type { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Environment Environment { get; set; } = null!;
}

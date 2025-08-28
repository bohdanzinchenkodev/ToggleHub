namespace ToggleHub.Application.DTOs.ApiKey;

public class ApiKeyDto
{
    public string Key { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public int OrganizationId { get; set; }
}
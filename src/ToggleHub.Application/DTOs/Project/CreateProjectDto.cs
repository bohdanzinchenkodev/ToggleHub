namespace ToggleHub.Application.DTOs.Project;

public class CreateProjectDto
{
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
}
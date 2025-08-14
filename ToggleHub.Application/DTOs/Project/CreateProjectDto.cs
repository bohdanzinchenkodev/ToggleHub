namespace ToggleHub.Application.DTOs.Project;

public class CreateProjectDto
{
    public int OrgId { get; set; }
    public string Name { get; set; } = string.Empty;
}
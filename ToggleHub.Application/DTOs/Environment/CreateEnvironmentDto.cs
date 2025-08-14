using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Environment;

public class CreateEnvironmentDto
{
    public EnvironmentType Type { get; set; }
    public int ProjectId { get; set; }
}

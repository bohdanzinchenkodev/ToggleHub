using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Environment;

public class EnvironmentDto
{
    public int Id { get; set; }
    public EnvironmentType Type { get; set; }
    public int ProjectId { get; set; }
}

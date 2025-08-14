using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Environment;

public class UpdateEnvironmentDto
{
    public int Id { get; set; }
    public EnvironmentType Type { get; set; }
}

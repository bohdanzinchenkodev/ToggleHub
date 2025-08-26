using System.Text.Json.Serialization;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Environment;

public class CreateEnvironmentDto
{
    [JsonPropertyName("type")]
    public string TypeString { get; set; } = string.Empty;
    [JsonIgnore]
    public EnvironmentType? Type => TypeString.ParseEnum<EnvironmentType>();
    public int ProjectId { get; set; }
}

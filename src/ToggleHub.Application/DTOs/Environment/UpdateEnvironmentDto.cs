using System.Text.Json.Serialization;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Environment;

public class UpdateEnvironmentDto
{
    public int Id { get; set; }
    [JsonIgnore]
    
    public EnvironmentType? Type => TypeString.ParseEnum<EnvironmentType>();
    [JsonPropertyName("type")]
    public string TypeString { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Environment;

public class EnvironmentDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public EnvironmentType Type { get; set; }
    [JsonPropertyName("type")]
    public string TypeString => Type.ParseEnumToString();
}

using System.Text.Json.Serialization;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Organization;

public class OrgMemberDto
{
    public int Id { get; set; }
    public int OrgId { get; set; }
    [JsonIgnore]
    public OrgMemberRole OrgRole { get; set; }
    [JsonPropertyName("orgRole")]
    public string OrgRoleString => OrgRole.ToString();
    public UserDto User { get; set; }
}
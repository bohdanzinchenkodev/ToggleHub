using System.Text.Json.Serialization;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Organization;

public class ChangeOrgMemberRoleDto
{
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    [JsonPropertyName("newRole")]
    public string? NewRoleString { get; set; }
    [JsonIgnore]
    public OrgMemberRole? NewRole => NewRoleString?.ParseEnum<OrgMemberRole>();
}

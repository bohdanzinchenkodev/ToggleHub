using System.Text.Json.Serialization;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Organization;

public class OrgMemberDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    [JsonIgnore]
    public OrgMemberRole OrganizationRole { get; set; }
    [JsonPropertyName("orgRole")]
    public string OrgRoleString => OrganizationRole.ToString();
    public UserDto User { get; set; }
}
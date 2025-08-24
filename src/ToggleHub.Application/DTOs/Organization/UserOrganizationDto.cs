using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Organization;

public class UserOrganizationDto
{
    public int OrgMemberId { get; set; }
    public int OrgId { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public string OrgSlug { get; set; } = string.Empty;
    public OrgMemberRole Role { get; set; }
}
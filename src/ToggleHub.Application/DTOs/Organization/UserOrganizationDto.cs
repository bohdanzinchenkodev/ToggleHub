using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Organization;

public class UserOrganizationDto
{
    public int OrgMemberId { get; set; }
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationSlug { get; set; } = string.Empty;
    public OrgMemberRole Role { get; set; }
}
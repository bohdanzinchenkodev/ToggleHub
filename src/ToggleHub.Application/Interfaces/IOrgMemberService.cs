using ToggleHub.Application.DTOs.Organization;

namespace ToggleHub.Application.Interfaces;

public interface IOrgMemberService
{
    Task AddUserToOrganizationAsync(int organizationId, int userId);
    Task RemoveUserFromOrganizationAsync(int organizationId, int userId);
    Task<IEnumerable<OrgMemberDto>> GetMembersInOrganizationAsync(int organizationId);
    Task<OrgMemberDto?> GetOrgMemberAsync(int organizationId, int userId);
    Task<bool> IsUserInOrganizationAsync(int organizationId, int userId);
}
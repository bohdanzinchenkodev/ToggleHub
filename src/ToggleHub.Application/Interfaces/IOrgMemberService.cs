using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;

namespace ToggleHub.Application.Interfaces;

public interface IOrgMemberService
{
    Task AddUserToOrganizationAsync(int organizationId, int userId);
    Task RemoveUserFromOrganizationAsync(int organizationId, int userId);
    Task RemoveOrgMemberAsync(int orgMemberId);
    Task<PagedListDto<OrgMemberDto>> GetMembersInOrganizationAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<OrgMemberDto?> GetOrgMemberAsync(int organizationId, int userId);
    Task<bool> IsUserInOrganizationAsync(int organizationId, int userId);
    Task ChangeOrgMemberRoleAsync(ChangeOrgMemberRoleDto dto);
}
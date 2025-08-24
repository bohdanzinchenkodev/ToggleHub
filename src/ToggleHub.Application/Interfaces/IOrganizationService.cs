using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.DTOs.User;

namespace ToggleHub.Application.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationDto> CreateAsync(CreateOrganizationDto createDto);
    Task UpdateAsync(UpdateOrganizationDto updateDto);
    Task<OrganizationDto?> GetByIdAsync(int id);
    Task<OrganizationDto?> GetBySlugAsync(string slug);
    Task DeleteAsync(int id);
    Task AddUserToOrganizationAsync(int organizationId, int userId);
    Task RemoveUserFromOrganizationAsync(int organizationId, int userId);
    Task<IEnumerable<OrgMemberDto>> GetMembersInOrganizationAsync(int organizationId);
    Task<OrgMemberDto?> GetOrgMemberAsync(int organizationId, int userId);
    Task<bool> IsUserInOrganizationAsync(int organizationId, int userId);
}
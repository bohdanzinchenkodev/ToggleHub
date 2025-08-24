using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization>
{
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsAsync(string name, int excludeId);
    Task AddOrgMemberAsync(OrgMember orgMember);
    Task DeleteOrgMember(OrgMember orgMember);
    Task<IEnumerable<OrgMember>> GetMembersInOrganizationAsync(int organizationId);
    Task<bool> IsUserInOrganizationAsync(int organizationId, int userId);
    Task<OrgMember?> GetOrgMemberAsync(int organizationId, int userId);
}

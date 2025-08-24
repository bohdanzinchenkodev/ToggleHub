using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IOrgMemberRepository : IBaseRepository<OrgMember>
{
    Task AddOrgMemberAsync(OrgMember orgMember);
    Task DeleteOrgMember(OrgMember orgMember);
    Task<IEnumerable<OrgMember>> GetMembersInOrganizationAsync(int organizationId);
    Task<bool> IsUserInOrganizationAsync(int organizationId, int userId);
    Task<OrgMember?> GetOrgMemberAsync(int organizationId, int userId);
}

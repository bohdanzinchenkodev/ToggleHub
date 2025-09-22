using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IOrganizationInviteRepository : IBaseRepository<OrganizationInvite>
{
    Task<OrganizationInvite?> GetByTokenAsync(string token);
    Task<OrganizationInvite?> GetByEmailAndOrganizationIdAsync(string email, int organizationId);
    Task<IPagedList<OrganizationInvite>> GetByOrganizationIdAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<List<OrganizationInvite>> GetExpiredInvitesAsync();
    Task<bool> HasPendingInviteAsync(string email, int organizationId);
    Task MarkAsExpiredAsync(int inviteId);
    Task MarkAsAcceptedAsync(int inviteId);
    Task MarkAsDeclinedAsync(int inviteId);
    Task MarkAsRevokedAsync(int inviteId);
}

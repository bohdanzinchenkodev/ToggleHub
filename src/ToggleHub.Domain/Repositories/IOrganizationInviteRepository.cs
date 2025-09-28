using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IOrganizationInviteRepository : IBaseRepository<OrganizationInvite>
{
    Task<OrganizationInvite?> GetByTokenAsync(string token);
    Task<IPagedList<OrganizationInvite>> GetByOrganizationIdAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<List<OrganizationInvite>> GetExpiredInvitesAsync();
    Task<bool> HasPendingInviteAsync(string email, int organizationId);
}

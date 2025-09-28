using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class OrganizationInviteRepository : BaseRepository<OrganizationInvite>, IOrganizationInviteRepository
{
    public OrganizationInviteRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
    }

    protected override IQueryable<OrganizationInvite> WithIncludes(DbSet<OrganizationInvite> dbSet)
    {
        return dbSet.Include(i => i.Organization);
    }
    public async Task<OrganizationInvite?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.Token == token);
    }

    public async Task<OrganizationInvite?> GetByEmailAndOrganizationIdAsync(string email, int organizationId)
    {
        return await _dbSet
            .Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.Email.ToLower() == email.ToLower() && i.OrganizationId == organizationId);
    }

    public async Task<IPagedList<OrganizationInvite>> GetByOrganizationIdAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _dbSet
            .Include(i => i.Organization)
            .Where(i => i.OrganizationId == organizationId)
            .OrderByDescending(i => i.CreatedAt);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<List<OrganizationInvite>> GetExpiredInvitesAsync()
    {
        return await _dbSet
            .Where(i => i.Status == InviteStatus.Pending && i.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<bool> HasPendingInviteAsync(string email, int organizationId)
    {
        return await _dbSet
            .AnyAsync(i => i.Email.ToLower() == email.ToLower() 
                          && i.OrganizationId == organizationId 
                          && i.Status == InviteStatus.Pending
                          && i.ExpiresAt > DateTime.UtcNow);
    }
}

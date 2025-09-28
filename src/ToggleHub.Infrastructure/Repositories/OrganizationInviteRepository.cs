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
    private readonly ICacheManager _cacheManager;
    private readonly IRepositoryCacheKeyFactory _repositoryCacheKeyFactory;

    public OrganizationInviteRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _repositoryCacheKeyFactory = cacheKeyFactory;
    }

    protected override IQueryable<OrganizationInvite> WithIncludes(DbSet<OrganizationInvite> dbSet)
    {
        return dbSet.Include(i => i.Organization);
    }
    public async Task<OrganizationInvite?> GetByTokenAsync(string token)
    {
        var cacheKey = _repositoryCacheKeyFactory.For<OrganizationInvite>(new Dictionary<string, object?>
        {
            { nameof(token), token }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await WithIncludes(_dbSet)
                .FirstOrDefaultAsync(i => i.Token == token);
        });
    }
    

    public async Task<IPagedList<OrganizationInvite>> GetByOrganizationIdAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var cacheKey = _repositoryCacheKeyFactory.For<OrganizationInvite>(new Dictionary<string, object?>
        {
            { nameof(organizationId), organizationId },
            { nameof(pageIndex), pageIndex },
            { nameof(pageSize), pageSize }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var query = WithIncludes(_dbSet)
                .Where(i => i.OrganizationId == organizationId)
                .OrderByDescending(i => i.CreatedAt);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        });
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

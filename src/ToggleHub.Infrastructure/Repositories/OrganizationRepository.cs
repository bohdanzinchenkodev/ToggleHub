using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class OrganizationRepository : BaseSluggedRepository<Organization>, IOrganizationRepository
{
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyFactory _cacheKeyFactory;

    public OrganizationRepository(ToggleHubDbContext context, ICacheManager cacheManager, ICacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _cacheKeyFactory = cacheKeyFactory;
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> NameExistsAsync(string name, int excludeId)
    {
        // Don't cache this method as it's context-specific with excludeId
        // Each call with different excludeId would need separate cache entry
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower() && o.Id != excludeId);
    }

    public async Task<IPagedList<Organization>> GetOrganizationsByUserIdAsync(int userId, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var cacheKey = _cacheKeyFactory.For<Organization>(new Dictionary<string, object?>
        {
            { nameof(userId), userId },
            { nameof(pageIndex), pageIndex },
            { nameof(pageSize), pageSize }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var query = _context.OrgMembers
                .Include(x => x.Organization)
                .Where(x => x.UserId == userId)
                .Select(x => x.Organization)
                .OrderByDescending(x => x.Id)
                .Distinct();
            
            return await query.ToPagedListAsync(pageIndex, pageSize);
        });
    }

}

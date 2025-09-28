using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class ApiKeyRepository : BaseRepository<ApiKey>, IApiKeyRepository
{
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyFactory _cacheKeyFactory;
    public ApiKeyRepository(ToggleHubDbContext context, ICacheManager cacheManager, ICacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _cacheKeyFactory = cacheKeyFactory;
    }

    public async Task<ApiKey?> GetByKeyAsync(string key)
    {
        var cacheKey = _cacheKeyFactory.For<ApiKey>(new Dictionary<string, object?>
        {
            { nameof(key), key }
        });
        return await _cacheManager.GetAsync(
            cacheKey,
            async () =>
            {
                var query = WithIncludes(_dbSet).AsNoTracking();
                return await query.FirstOrDefaultAsync(a => a.Key == key);
            });
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await GetByKeyAsync(key) != null;
    }

    public async Task<IPagedList<ApiKey>> GetApiKeysAsync(int organizationId, int projectId, int environmentId, int pageNumber, int pageSize)
    {
        var cacheKey = _cacheKeyFactory.For<ApiKey>(new Dictionary<string, object?>
        {
            { nameof(organizationId), organizationId },
            { nameof(projectId), projectId },
            { nameof(environmentId), environmentId },
            { nameof(pageNumber), pageNumber },
            { nameof(pageSize), pageSize }
        });
        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var query = _dbSet.AsQueryable();

            if (organizationId > 0)
                query = query.Where(a => a.OrganizationId == organizationId);

            if (projectId > 0)
                query = query.Where(a => a.ProjectId == projectId);

            if (environmentId > 0)
                query = query.Where(a => a.EnvironmentId == environmentId);
            
            query = query.OrderByDescending(x => x.Id);

            return await query.ToPagedListAsync(pageNumber, pageSize);
        });
    }
}

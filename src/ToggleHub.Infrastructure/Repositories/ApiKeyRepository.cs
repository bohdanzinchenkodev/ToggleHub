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
    public ApiKeyRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
    }

    public async Task<ApiKey?> GetByKeyAsync(string key)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Key == key);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _dbSet.AnyAsync(a => a.Key == key);
    }

    public Task<IPagedList<ApiKey>> GetApiKeysAsync(int organizationId, int projectId, int environmentId, int pageNumber, int pageSize)
    {
        var query = _dbSet.AsQueryable();

        if (organizationId > 0)
            query = query.Where(a => a.OrganizationId == organizationId);

        if (projectId > 0)
            query = query.Where(a => a.ProjectId == projectId);

        if (environmentId > 0)
            query = query.Where(a => a.EnvironmentId == environmentId);

        return query.ToPagedListAsync(pageNumber, pageSize);
    }

    public async Task<IEnumerable<ApiKey>> GetByProjectIdAsync(int projectId)
    {
        return await _dbSet.Where(a => a.ProjectId == projectId).ToListAsync();
    }

    public async Task<bool> ApiKeyExistsForEnvironmentAsync(int projectId, int environmentId)
    {
        return await _dbSet.AnyAsync(x => x.ProjectId == projectId && x.EnvironmentId == environmentId);
    }
}

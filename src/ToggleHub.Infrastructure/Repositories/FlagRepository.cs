using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class FlagRepository : BaseRepository<Flag>, IFlagRepository
{
    private readonly ICacheManager _cacheManager;
    private readonly IRepositoryCacheKeyFactory _repositoryCacheKeyFactory;

    public FlagRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _repositoryCacheKeyFactory = cacheKeyFactory;
    }

    protected override IQueryable<Flag> WithIncludes(DbSet<Flag> dbSet)
    {
        return dbSet.Include(x => x.RuleSets)
            .ThenInclude(x => x.Conditions)
            .ThenInclude(x => x.Items);
    }

    public async Task<bool> ExistsAsync(string key, int environmentId, int projectId)
    {
        return await GetFlagByKeyAsync(key, environmentId, projectId) != null;
    }

    public async Task<Flag?> GetFlagByKeyAsync(string key, int environmentId, int projectId)
    {
        var cacheKey = _repositoryCacheKeyFactory.For<Flag>(new Dictionary<string, object?>
        {
            { nameof(key), key },
            { nameof(environmentId), environmentId },
            { nameof(projectId), projectId }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await WithIncludes(_dbSet)
                .FirstOrDefaultAsync(x => x.Key == key && x.EnvironmentId == environmentId && x.ProjectId == projectId);
        });
    }

    public async Task<IPagedList<Flag>> GetAllAsync(int? projectId = null, int? environmentId = null, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var cacheKey = _repositoryCacheKeyFactory.For<Flag>(new Dictionary<string, object?>
        {
            { nameof(projectId), projectId },
            { nameof(environmentId), environmentId },
            { nameof(pageIndex), pageIndex },
            { nameof(pageSize), pageSize }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var query = _dbSet
                .AsQueryable();

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);
            
            if (environmentId.HasValue)
                query = query.Where(x => x.EnvironmentId == environmentId.Value);

            query = query.OrderByDescending(x => x.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        });
    }
}

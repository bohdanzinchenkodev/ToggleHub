using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Infrastructure.Repositories;

public class EnvironmentRepository : BaseRepository<Environment>, IEnvironmentRepository
{
    private readonly ICacheManager _cacheManager;
    private readonly IRepositoryCacheKeyFactory _repositoryCacheKeyFactory;

    public EnvironmentRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _repositoryCacheKeyFactory = cacheKeyFactory;
    }

    public async Task<IPagedList<Environment>> GetAllAsync(int? projectId = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var cacheKey = _repositoryCacheKeyFactory.For<Environment>(new Dictionary<string, object?>
        {
            { nameof(projectId), projectId },
            { nameof(pageIndex), pageIndex },
            { nameof(pageSize), pageSize }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var query = _dbSet.AsQueryable();
            if (projectId.HasValue)
                query = _dbSet.Where(e => e.ProjectId == projectId.Value);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        });
    }

    public async Task<bool> EnvironmentExistsAsync(EnvironmentType type, int projectId)
    {
        var cacheKey = _repositoryCacheKeyFactory.For<Environment>(new Dictionary<string, object?>
        {
            { nameof(type), type },
            { nameof(projectId), projectId }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await _dbSet.AnyAsync(e => e.Type == type && e.ProjectId == projectId);
        });
    }
}

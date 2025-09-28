using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class BaseSluggedRepository<T> : BaseRepository<T>, IBaseSluggedRepository<T> 
    where T : BaseEntity, ISluggedEntity
{
    private readonly ICacheManager _cacheManager;
    private readonly IRepositoryCacheKeyFactory _repositoryCacheKeyFactory;
    public BaseSluggedRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory, IRepositoryCacheKeyFactory repositoryCacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _repositoryCacheKeyFactory = repositoryCacheKeyFactory;
    }

    public virtual async Task<T?> GetBySlugAsync(string slug)
    {
        var key = _repositoryCacheKeyFactory.ForSlug<T>(slug);
        return await _cacheManager.GetAsync(
            key,
            async () =>
            {
                var query = WithIncludes(_dbSet).AsNoTracking();
                return await query
                    .FirstOrDefaultAsync(entity => entity.Slug == slug);
            });
    }
    
}

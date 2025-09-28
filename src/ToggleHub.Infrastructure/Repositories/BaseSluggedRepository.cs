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
    private readonly ICacheKeyFactory _cacheKeyFactory;
    public BaseSluggedRepository(ToggleHubDbContext context, ICacheManager cacheManager, ICacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _cacheKeyFactory = cacheKeyFactory;
    }

    public virtual async Task<T?> GetBySlugAsync(string slug)
    {
        var key = _cacheKeyFactory.ForSlug<T>(slug);
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

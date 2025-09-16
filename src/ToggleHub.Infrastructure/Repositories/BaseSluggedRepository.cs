using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class BaseSluggedRepository<T> : BaseRepository<T>, IBaseSluggedRepository<T> 
    where T : BaseEntity, ISluggedEntity
{
    public BaseSluggedRepository(ToggleHubDbContext context) : base(context)
    {
    }

    public virtual async Task<T?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Slug == slug);
    }

    public virtual async Task<IEnumerable<string>> GetSlugsByPatternAsync(string baseSlug)
    {
        return await _dbSet
            .Where(entity => entity.Slug == baseSlug || entity.Slug.StartsWith(baseSlug + "-"))
            .Select(entity => entity.Slug)
            .ToListAsync();
    }
}

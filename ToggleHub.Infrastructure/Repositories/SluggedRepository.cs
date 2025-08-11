using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class SluggedRepository<T> : ISluggedRepository<T> where T : BaseEntity, ISluggedEntity
{
    private readonly DbSet<T> _dbSet;

    public SluggedRepository(ToggleHubDbContext context)
    {
        _dbSet = context.Set<T>();
    }

    public Task<T?> GetBySlugAsync(string slug)
    {
        return _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Slug == slug);
    }

    public async Task<IEnumerable<string>> GetSlugsByPatternAsync(string baseSlug)
    {
        return await _dbSet
            .Where(o => o.Slug == baseSlug || o.Slug.StartsWith(baseSlug + "-"))
            .Select(o => o.Slug)
            .ToListAsync();
    }
}
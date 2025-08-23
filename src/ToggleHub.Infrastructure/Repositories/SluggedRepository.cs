using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class SluggedRepository : ISluggedRepository
{
    private readonly ToggleHubDbContext _context;

    public SluggedRepository(ToggleHubDbContext context)
    {
        _context = context;
    }

    public Task<T?> GetBySlugAsync<T>(string slug) where T : BaseEntity, ISluggedEntity
    {
        var dbSet = _context.Set<T>();
        return dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Slug == slug);
    }

    public async Task<IEnumerable<string>> GetSlugsByPatternAsync<T>(string baseSlug) where T : BaseEntity, ISluggedEntity
    {
        var dbSet = _context.Set<T>();
        return await dbSet
            .Where(o => o.Slug == baseSlug || o.Slug.StartsWith(baseSlug + "-"))
            .Select(o => o.Slug)
            .ToListAsync();
    }
}
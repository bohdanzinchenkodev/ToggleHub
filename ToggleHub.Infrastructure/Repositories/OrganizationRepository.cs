using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ToggleHubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<string>> GetSlugsByPatternAsync(string baseSlug)
    {
        return await _dbSet
            .Where(o => o.Slug == baseSlug || o.Slug.StartsWith(baseSlug + "-"))
            .Select(o => o.Slug)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> NameExistsAsync(string name, int excludeId)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower() && o.Id != excludeId);
    }
}

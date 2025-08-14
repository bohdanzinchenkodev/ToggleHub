using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(ToggleHubDbContext context) : base(context)
    {
 
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

    public async Task<IEnumerable<Project>> GetAllAsync(int? organizationId = null)
    {
        var query = _dbSet.AsQueryable();

        if (organizationId.HasValue)
        {
            query = query.Where(p => p.OrgId == organizationId.Value);
        }

        return await query.ToListAsync();
    }
}

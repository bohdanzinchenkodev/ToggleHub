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

    public async Task<bool> NameExistsAsync(string name, int organizationId = 0)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower() && (organizationId == 0 || o.OrganizationId == organizationId));
    }

    public async Task<IEnumerable<Project>> GetAllAsync(int? organizationId = null)
    {
        var query = _dbSet.AsQueryable();

        if (organizationId.HasValue)
        {
            query = query.Where(p => p.OrganizationId == organizationId.Value);
        }

        return await query.ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Infrastructure.Repositories;

public class EnvironmentRepository : BaseRepository<Environment>, IEnvironmentRepository
{
    public EnvironmentRepository(ToggleHubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Environment>> GetAllAsync(int? organizationId = null)
    {
        var query = _dbSet.AsQueryable();
        if (organizationId.HasValue)
            query = _dbSet.Where(e => e.OrganizationId == organizationId.Value);

        return await query.ToListAsync();
    }
}

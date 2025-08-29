using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Infrastructure.Repositories;

public class EnvironmentRepository : BaseRepository<Environment>, IEnvironmentRepository
{
    public EnvironmentRepository(ToggleHubDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Environment>> GetAllAsync(int? projectId = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _dbSet.AsQueryable();
        if (projectId.HasValue)
            query = _dbSet.Where(e => e.ProjectId == projectId.Value);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public Task<bool> EnvironmentExistsAsync(EnvironmentType type, int projectId)
    {
        return _dbSet.AnyAsync(e => e.Type == type && e.ProjectId == projectId);
    }
}

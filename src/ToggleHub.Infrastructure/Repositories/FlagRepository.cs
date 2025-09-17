using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class FlagRepository : BaseRepository<Flag>, IFlagRepository
{
    public FlagRepository(ToggleHubDbContext context) : base(context)
    {
    }

    public override async Task<Flag?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(x => x.RuleSets)
            .ThenInclude(x => x.Conditions)
            .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
            
    }

    public Task<bool> ExistsAsync(string key, int environmentId, int projectId)
    {
        return _dbSet
            .AnyAsync(x => x.Key == key && x.EnvironmentId == environmentId && x.ProjectId == projectId);
    }

    public Task<Flag?> GetFlagByKeyAsync(string key, int environmentId, int projectId)
    {
        return _dbSet
            .Include(x => x.RuleSets)
            .ThenInclude(x => x.Conditions)
            .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.Key == key && x.EnvironmentId == environmentId && x.ProjectId == projectId);
    }

    public Task<IPagedList<Flag>> GetAllAsync(int? projectId = null, int? environmentId = null, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var query = _dbSet
            .AsQueryable();

        if (projectId.HasValue)
            query = query.Where(x => x.ProjectId == projectId.Value);
        
        if (environmentId.HasValue)
            query = query.Where(x => x.EnvironmentId == environmentId.Value);

        return query.ToPagedListAsync(pageIndex, pageSize);
    }
}

using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

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
}

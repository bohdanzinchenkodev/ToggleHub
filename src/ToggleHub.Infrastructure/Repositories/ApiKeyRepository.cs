using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class ApiKeyRepository : BaseRepository<ApiKey>, IApiKeyRepository
{
    public ApiKeyRepository(ToggleHubDbContext context) : base(context)
    {
    }
    public async Task<ApiKey?> GetByKeyAsync(string key)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Key == key);
    }

    public Task<bool> KeyExistsAsync(string key)
    {
        return _dbSet.AnyAsync(a => a.Key == key);
    }

    public async Task<IEnumerable<ApiKey>> GetByProjectIdAsync(int projectId)
    {
        return await _dbSet.Where(a => a.ProjectId == projectId).ToListAsync();
    }
}

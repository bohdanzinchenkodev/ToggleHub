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
}

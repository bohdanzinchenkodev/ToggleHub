using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IApiKeyRepository : IBaseRepository<ApiKey>
{
    Task<ApiKey?> GetByKeyAsync(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<IEnumerable<ApiKey>> GetByProjectIdAsync(int projectId);
    
}

using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IApiKeyRepository : IBaseRepository<ApiKey>
{
    Task<ApiKey?> GetByKeyAsync(string key);
}

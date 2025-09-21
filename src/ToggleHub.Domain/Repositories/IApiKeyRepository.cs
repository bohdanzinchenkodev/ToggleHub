using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IApiKeyRepository : IBaseRepository<ApiKey>
{
    Task<ApiKey?> GetByKeyAsync(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<IPagedList<ApiKey>> GetApiKeysAsync(int organizationId, int projectId, int environmentId, int pageNumber, int pageSize);

}

using ToggleHub.Application.DTOs.ApiKey;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

public class ApiKeyService : IApiKeyService
{
    public Task CreateApiKeyAsync(int projectId, int environmentId, int organizationId)
    {
        throw new NotImplementedException();
    }

    public Task CreateApiKeysForProjectAsync(int projectId, int organizationId)
    {
        throw new NotImplementedException();
    }

    public Task RevokeApiKeyAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task RevokeApiKeysForProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiKeyDto> GetByKeyAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ApiKeyDto>> GetAllAsync(int? projectId = null)
    {
        throw new NotImplementedException();
    }
}
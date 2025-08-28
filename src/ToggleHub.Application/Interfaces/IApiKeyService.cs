using ToggleHub.Application.DTOs.ApiKey;

namespace ToggleHub.Application.Interfaces;

public interface IApiKeyService
{
    Task CreateApiKeyAsync(int projectId, int environmentId, int organizationId);
    Task CreateApiKeysForProjectAsync(int projectId, int organizationId);
    Task RevokeApiKeyAsync(string key);
    Task RevokeApiKeysForProjectAsync(int projectId);
    Task<ApiKeyDto?> GetByKeyAsync(string key);
    Task<IEnumerable<ApiKeyDto>> GetByProject(int projectId);
}
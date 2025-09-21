using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.ApiKey;
using Environment = ToggleHub.Domain.Entities.Environment;
namespace ToggleHub.Application.Interfaces;

public interface IApiKeyService
{
    Task CreateApiKeysForEnvironmentsAsync(IEnumerable<Environment> environments, int projectId);
    Task<ApiKeyDto?> GetByKeyAsync(string key);
    Task<PagedListDto<ApiKeyDto>> GetApiKeysAsync(int organizationId, int projectId, int environmentId, int pageNumber, int pageSize);
}
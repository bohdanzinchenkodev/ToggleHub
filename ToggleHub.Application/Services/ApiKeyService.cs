using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class ApiKeyService : BaseService<ApiKey>
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyService(IApiKeyRepository apiKeyRepository) 
        : base(apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }
}

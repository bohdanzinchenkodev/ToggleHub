using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.ApiKey;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyGenerator _apiKeyGenerator;
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IProjectRepository _projectRepository;

    public ApiKeyService(IApiKeyGenerator apiKeyGenerator, IApiKeyRepository apiKeyRepository, IProjectRepository projectRepository)
    {
        _apiKeyGenerator = apiKeyGenerator;
        _apiKeyRepository = apiKeyRepository;
        _projectRepository = projectRepository;
    }
    
    public async Task CreateApiKeysForEnvironmentsAsync(IEnumerable<Environment> environments, int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new ApplicationException($"Project with ID {projectId} not found.");
        
        var apiKeys = new List<ApiKey>();
        foreach (var env in environments)
        {
            var apiKey = await PrepareApiKeyEntityAsync(env.ProjectId, env.Id, project.OrganizationId);
            apiKeys.Add(apiKey);
        }
        await _apiKeyRepository.CreateAsync(apiKeys);
    }
    public async Task<ApiKeyDto?> GetByKeyAsync(string key)
    {
        var apiKey = await _apiKeyRepository.GetByKeyAsync(key);
        return apiKey?.ToDto();
    }

    public async Task<PagedListDto<ApiKeyDto>> GetApiKeysAsync(int organizationId, int projectId, int environmentId, int pageNumber, int pageSize)
    {
        var list = await _apiKeyRepository.GetApiKeysAsync(organizationId, projectId, environmentId, pageNumber, pageSize);
        var dtos = list.Select(ak => ak.ToDto()).ToList();
        return new PagedListDto<ApiKeyDto>(dtos, list.TotalCount, list.PageIndex, list.PageSize);
    }

    private async Task<ApiKey> PrepareApiKeyEntityAsync(int projectId, int environmentId, int organizationId)
    {
        var key = await GenerateUniqueKeyAsync();
        var apiKey = new ApiKey
        {
            Key = key,
            ProjectId = projectId,
            EnvironmentId = environmentId,
            OrganizationId = organizationId,
            ExpiresAt = DateTime.UtcNow.AddYears(1),
            IsActive = true,
        };
        return apiKey;
    }
    private async Task<string> GenerateUniqueKeyAsync()
    {
        string key;
        bool exists;
        int attempts = 0;
        do
        {
            key = await _apiKeyGenerator.GenerateKeyAsync();
            exists = await _apiKeyRepository.KeyExistsAsync(key);
            attempts++;
        } while (exists && attempts < 10);

        if (attempts >= 10)
            throw new InvalidOperationException("Failed to generate unique API key after multiple attempts.");

        return key;
    }
}
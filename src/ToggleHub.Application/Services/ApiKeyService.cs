using ToggleHub.Application.DTOs.ApiKey;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyGenerator _apiKeyGenerator;
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IEnvironmentRepository _environmentRepository;

    public ApiKeyService(IApiKeyGenerator apiKeyGenerator, IApiKeyRepository apiKeyRepository, IEnvironmentRepository environmentRepository)
    {
        _apiKeyGenerator = apiKeyGenerator;
        _apiKeyRepository = apiKeyRepository;
        _environmentRepository = environmentRepository;
    }

    public async Task CreateApiKeyAsync(int projectId, int environmentId, int organizationId)
    {
        var apiKey = await PrepareApiKeyEntityAsync(projectId, environmentId, organizationId);
        await _apiKeyRepository.CreateAsync(apiKey);
    }

    public async Task CreateApiKeysForProjectAsync(int projectId, int organizationId)
    {
        var envs = await _environmentRepository.GetAllAsync(projectId);
        var apiKeys = new List<ApiKey>();
        foreach (var env in envs)
        {
            var apiKey = await PrepareApiKeyEntityAsync(projectId, env.Id, organizationId);
            apiKeys.Add(apiKey);
        }
        await _apiKeyRepository.CreateAsync(apiKeys);
    }

    public async Task RevokeApiKeyAsync(string key)
    {
        var apiKey = await _apiKeyRepository.GetByKeyAsync(key);
        if (apiKey == null)
            throw new NotFoundException($"API key '{key}' not found.");
        
        apiKey.IsActive = false;
        await _apiKeyRepository.UpdateAsync(apiKey);
    }

    public async Task RevokeApiKeysForProjectAsync(int projectId)
    {
        var apiKeys = (await _apiKeyRepository.GetByProjectIdAsync(projectId)).ToArray();
        foreach (var apiKey in apiKeys)
        {
            apiKey.IsActive = false;
        }
        await _apiKeyRepository.UpdateAsync(apiKeys);
    }

    public async Task<ApiKeyDto?> GetByKeyAsync(string key)
    {
        var apiKey = await _apiKeyRepository.GetByKeyAsync(key);
        return apiKey?.ToDto();
    }

    public async Task<IEnumerable<ApiKeyDto>> GetByProject(int projectId)
    {
        return (await _apiKeyRepository.GetByProjectIdAsync(projectId))
            .Select(k => k.ToDto());
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
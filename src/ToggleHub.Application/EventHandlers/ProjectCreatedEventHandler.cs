using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.EventHandlers;

public class ProjectCreatedEventHandler : IConsumer<EntityCreatedEvent<Project>>,
    IConsumer<EntitiesCreatedEvent<Environment>>
{
    private readonly IEnvironmentService _environmentService;
    private readonly IApiKeyService _apiKeyService;

    public ProjectCreatedEventHandler(IEnvironmentService environmentService, IApiKeyService apiKeyService)
    {
        _environmentService = environmentService;
        _apiKeyService = apiKeyService;
    }

    public async Task HandleEventAsync(EntityCreatedEvent<Project> eventMessage)
    {
        var project = eventMessage.Entity;
        await _environmentService.GenerateMissingEnvironmentsForProjectAsync(project.Id);
        
    }

    public async Task HandleEventAsync(EntitiesCreatedEvent<Environment> eventMessage)
    {
        var environments = eventMessage.Entities.ToList();
        if (!environments.Any())
            return;
        
        var project = environments.First().Project;
        
        await _apiKeyService.CreateApiKeysForEnvironmentsAsync(environments, project.Id);
    }
}
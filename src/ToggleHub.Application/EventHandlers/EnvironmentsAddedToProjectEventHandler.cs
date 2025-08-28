using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Events;

namespace ToggleHub.Application.EventHandlers;

public class EnvironmentsAddedToProjectEventHandler : IConsumer<EnvironmentsAddedToProjectEvent>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IProjectService _projectService;

    public EnvironmentsAddedToProjectEventHandler(IApiKeyService apiKeyService, IProjectService projectService)
    {
        _apiKeyService = apiKeyService;
        _projectService = projectService;
    }

    public async Task HandleEventAsync(EnvironmentsAddedToProjectEvent eventMessage)
    {
        var projectId = eventMessage.ProjectId;
        var environments = eventMessage.Environments;
        await _apiKeyService.CreateApiKeysForEnvironmentsAsync(environments, projectId);
    }
}
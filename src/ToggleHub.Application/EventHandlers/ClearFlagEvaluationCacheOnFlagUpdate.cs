using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;

namespace ToggleHub.Application.EventHandlers;

public class ClearFlagEvaluationCacheOnFlagUpdate : IConsumer<FlagUpdatedEvent>,
    IConsumer<FlagDeletedEvent>
{
    private readonly IFlagEvaluationCacheManager _cacheManager;
    private readonly IProjectService _projectService;

    public ClearFlagEvaluationCacheOnFlagUpdate(IFlagEvaluationCacheManager cacheManager, IProjectService projectService)
    {
        _cacheManager = cacheManager;
        _projectService = projectService;
    }

    public async Task HandleEventAsync(FlagUpdatedEvent eventMessage)
    {
        await InvalidateCacheAsync(eventMessage.Flag);
    }

    public async Task HandleEventAsync(FlagDeletedEvent eventMessage)
    {
        await InvalidateCacheAsync(eventMessage.Flag);
    }
    private async Task InvalidateCacheAsync(Flag flag)
    {
        var project = await _projectService.GetByIdAsync(flag.ProjectId);
        if (project == null)
            return;
        
        await _cacheManager.InvalidateCacheAsync(project.OrganizationId, project.Id, flag.EnvironmentId, flag.Key);
    }
}
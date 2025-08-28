using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.EventHandlers;

public class ProjectCreatedEventHandler : IConsumer<ProjectCreatedEvent>
{
    private readonly IEnvironmentService _environmentService;

    public ProjectCreatedEventHandler(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    public async Task HandleEventAsync(ProjectCreatedEvent eventMessage)
    {
        var project = eventMessage.Project;
        await _environmentService.GenerateEnvironmentsForProjectAsync(project.Id);
    }
}
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.EventHandlers;

public class ProjectCreatedEventHandler : IConsumer<ProjectCreatedEvent>
{
    private readonly IEnvironmentRepository _environmentRepository;

    public ProjectCreatedEventHandler(IEnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }

    public async Task HandleEventAsync(ProjectCreatedEvent eventMessage)
    {
        var project = eventMessage.Project;
        // Create default environment for the new project
        var environment = new Environment
        {
            ProjectId = project.Id,
            Type = EnvironmentType.Prod
        };
        await _environmentRepository.CreateAsync(environment);
    }
}
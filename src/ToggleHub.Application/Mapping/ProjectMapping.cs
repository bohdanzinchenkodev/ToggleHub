using ToggleHub.Application.DTOs.Project;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class ProjectMapping
{
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
            OrganizationId = project.OrganizationId,
            CreatedAt = project.CreatedAt,
            Environments = project.Environments
                .Select(e => e.ToDto())
                .ToList()
        };
    }

    public static Project ToEntity(this CreateProjectDto createDto, Project? project = null)
    {
        project ??= new Project();
        project.Name = createDto.Name;
        project.OrganizationId = createDto.OrganizationId;
        return project;
    }
    
    public static Project ToEntity(this UpdateProjectDto updateDto, Project? project = null)
    {
        project ??= new Project();
        project.Id = updateDto.Id;
        project.Name = updateDto.Name;
        return project;
    }
}
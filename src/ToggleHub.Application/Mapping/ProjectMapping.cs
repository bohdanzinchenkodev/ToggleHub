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
            Slug = project.Slug
        };
    }

    public static Project ToEntity(this CreateProjectDto createDto, Project? project = null)
    {
        project ??= new Project();
        project.Name = createDto.Name;
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
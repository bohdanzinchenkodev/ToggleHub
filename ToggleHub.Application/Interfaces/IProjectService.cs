using ToggleHub.Application.DTOs.Project;

namespace ToggleHub.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto?> GetBySlugAsync(string slug);
    Task<ProjectDto> CreateAsync(CreateProjectDto createProjectDto);
    Task<ProjectDto> UpdateAsync(UpdateProjectDto updateProjectDto);
    Task DeleteAsync(int id);
    Task<IEnumerable<ProjectDto>> GetAllAsync(int? organizationId = null);
}
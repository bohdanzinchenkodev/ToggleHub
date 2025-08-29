using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Project;

namespace ToggleHub.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto?> GetBySlugAsync(string slug);
    Task<ProjectDto> CreateAsync(CreateProjectDto createProjectDto);
    Task<ProjectDto> UpdateAsync(UpdateProjectDto updateProjectDto);
    Task DeleteAsync(int id);
    Task<PagedListDto<ProjectDto>> GetAllAsync(int? organizationId = null, int pageIndex = 0, int pageSize = int.MaxValue);
}
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Environment;

namespace ToggleHub.Application.Interfaces;

public interface IEnvironmentService
{
	Task<EnvironmentDto> CreateAsync(CreateEnvironmentDto createDto);
	Task<EnvironmentDto> UpdateAsync(UpdateEnvironmentDto updateDto);
	Task<EnvironmentDto?> GetByIdAsync(int id);
	Task<PagedListDto<EnvironmentDto>> GetAllAsync(int? projectId = null, int pageIndex = 0, int pageSize = int.MaxValue);
	Task DeleteAsync(int id);
	Task<IEnumerable<EnvironmentTypeDto>> GetEnvironmentTypesAsync();
	Task GenerateMissingEnvironmentsForProjectAsync(int projectId);
}
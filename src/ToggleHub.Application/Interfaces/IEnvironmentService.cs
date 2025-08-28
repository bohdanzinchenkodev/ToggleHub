using ToggleHub.Application.DTOs.Environment;

namespace ToggleHub.Application.Interfaces;

public interface IEnvironmentService
{
	Task<EnvironmentDto> CreateAsync(CreateEnvironmentDto createDto);
	Task<EnvironmentDto> UpdateAsync(UpdateEnvironmentDto updateDto);
	Task<EnvironmentDto?> GetByIdAsync(int id);
	Task<IEnumerable<EnvironmentDto>> GetAllAsync(int? projectId = null);
	Task DeleteAsync(int id);
	Task<IEnumerable<EnvironmentTypeDto>> GetEnvironmentTypesAsync();
	Task GenerateEnvironmentsForProjectAsync(int projectId);
}
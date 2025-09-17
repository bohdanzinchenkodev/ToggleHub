using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;

namespace ToggleHub.Application.Interfaces;

public interface IFlagService
{
    Task<FlagDto> CreateAsync(CreateFlagDto createDto);
    Task<FlagDto?> GetByIdAsync(int id);
    Task<FlagDto> UpdateAsync(UpdateFlagDto updateDto);
    Task DeleteAsync(int id);
    Task<PagedListDto<FlagDto>> GetAllAsync(int? projectId = null, int? environmentId = null, int pageIndex = 0, int pageSize = int.MaxValue);
    Task SetEnabledAsync(int id, bool isEnabled);
}
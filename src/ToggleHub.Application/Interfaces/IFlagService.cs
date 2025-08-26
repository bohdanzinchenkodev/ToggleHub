using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;

namespace ToggleHub.Application.Interfaces;

public interface IFlagService
{
    Task<FlagDto> CreateAsync(CreateCreateOrUpdateFlagDto createCreateOrUpdateDto);
    Task<FlagDto?> GetByIdAsync(int id);
    Task<FlagDto> UpdateAsync(UpdateCreateOrUpdateFlagDto updateCreateOrUpdateDto);
    Task DeleteAsync(int id);
}
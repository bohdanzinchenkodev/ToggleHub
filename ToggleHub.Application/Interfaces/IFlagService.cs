using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;

namespace ToggleHub.Application.Interfaces;

public interface IFlagService
{
    Task<FlagDto> CreateAsync(CreateFlagDto createDto);
    Task<FlagDto?> GetByIdAsync(int id);
    Task<FlagDto> UpdateAsync(UpdateFlagDto updateDto);
}
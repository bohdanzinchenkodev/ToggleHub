using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.DTOs.User;

namespace ToggleHub.Application.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationDto> CreateAsync(CreateOrganizationDto createDto);
    Task UpdateAsync(UpdateOrganizationDto updateDto);
    Task<OrganizationDto?> GetByIdAsync(int id);
    Task<OrganizationDto?> GetBySlugAsync(string slug);
    Task DeleteAsync(int id);
    Task<PagedListDto<OrganizationDto>> GetOrganizationsByUserIdAsync(int userId, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<PagedListDto<OrganizationDto>> GetOrganizationsForCurrentUserAsync(int pageIndex = 0, int pageSize = int.MaxValue);
}
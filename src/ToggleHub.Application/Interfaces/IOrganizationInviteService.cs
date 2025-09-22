using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.OrganizationInvite;

namespace ToggleHub.Application.Interfaces;

public interface IOrganizationInviteService
{
    Task<OrganizationInviteDto> CreateInviteAsync(CreateOrganizationInviteDto createDto);
    Task<OrganizationInviteDto?> GetByIdAsync(int id);
    Task<OrganizationInviteDto?> GetByTokenAsync(string token);
    Task<PagedListDto<OrganizationInviteDto>> GetByOrganizationIdAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue);
    Task AcceptInviteAsync(AcceptInviteDto acceptDto);
    Task DeclineInviteAsync(string token);
    Task RevokeInviteAsync(int inviteId);
    Task ResendInviteAsync(int inviteId);
    Task ProcessExpiredInvitesAsync();
    Task<bool> HasPendingInviteAsync(string email, int organizationId);
}

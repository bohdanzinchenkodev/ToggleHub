using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Interfaces;

public interface IOrganizationPermissionService
{
    Task<bool> AuthorizeAsync(int organizationId, int userId, string permission);
    Task<bool> AuthorizeAsync(int organizationId, string permission);
    Task ThrowIfNotAuthorizedAsync(int organizationId, string permission);
    Task ThrowIfNotAuthorizedAsync(int organizationId, int userId, string permission);
}

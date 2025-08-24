using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Helpers;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationPermissionService : IOrganizationPermissionService
{
    private readonly IOrgMemberRepository _orgMemberRepository;
    private readonly IWorkContext _workContext;

    public OrganizationPermissionService(IOrgMemberRepository orgMemberRepository, IWorkContext workContext)
    {
        _orgMemberRepository = orgMemberRepository;
        _workContext = workContext;
    }

    public async Task<bool> AuthorizeAsync(int organizationId, int userId, string permission)
    {
        var orgMember = await _orgMemberRepository.GetOrgMemberAsync(organizationId, userId);
        if (orgMember == null)
            return false;
        
        var roles = PermissionRoleHelper.GetRolesByPermission(permission);
        return roles.Any(x => x == orgMember.Role);
    }

    public async Task<bool> AuthorizeAsync(int organizationId, string permission)
    {
        var userId = _workContext.GetCurrentUserId();
        if (!userId.HasValue)
            return false;
        
        return await AuthorizeAsync(organizationId, userId.Value, permission);
    }
    public async Task ThrowIfNotAuthorizedAsync(int organizationId, int userId, string permission)
    {
        var orgMember = await _orgMemberRepository.GetOrgMemberAsync(organizationId, userId);
        if (orgMember == null)
            throw new UnauthorizedAccessException();
        
        var roles = PermissionRoleHelper.GetRolesByPermission(permission);
        if (roles.All(x => x != orgMember.Role))
            throw new NoAccessPermissionException();
    }

    public async Task ThrowIfNotAuthorizedAsync(int organizationId, string permission)
    {
        var userId = _workContext.GetCurrentUserId();
        if (!userId.HasValue)
            throw new UnauthorizedAccessException();
        
        await ThrowIfNotAuthorizedAsync(organizationId, userId.Value, permission);
    }
}
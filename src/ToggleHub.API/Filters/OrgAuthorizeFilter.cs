using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.API.Filters;


public sealed class OrgAuthorizeAttribute : TypeFilterAttribute
{
    public OrgAuthorizeAttribute(string permission, string routeKey = "organizationId", string? slugRouteKey = null)
        : base(typeof(OrgAuthorizeFilter))
    {
        Arguments = [permission, routeKey, slugRouteKey];
    }
}

public sealed class OrgAuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly IOrganizationPermissionService _perm;
    private readonly IOrganizationService _orgService; // instead of repo
    private readonly string? _slugRouteKey;
    private readonly string _routeKey;
    private readonly string _permission;

    public OrgAuthorizeFilter(
        string permission,
        string routeKey,
        string? slugRouteKey,
        IOrganizationPermissionService perm,
        IOrganizationService orgService)
    {
        _permission = permission;
        _routeKey = routeKey;
        _slugRouteKey = slugRouteKey;
        _perm = perm;
        _orgService = orgService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext ctx)
    {
        var orgId = TryGetIntRouteValue(ctx.RouteData, _routeKey);

        if (!orgId.HasValue && !string.IsNullOrWhiteSpace(_slugRouteKey))
        {
            var slug = ctx.RouteData.Values[_slugRouteKey!]?.ToString();
            if (!string.IsNullOrWhiteSpace(slug))
            {
                var orgDto = await _orgService.GetBySlugAsync(slug);
                orgId = orgDto?.Id;
            }
        }

        if (!orgId.HasValue)
        {
            ctx.Result = new BadRequestObjectResult("Organization not found.");
            return;
        }

        if (!await _perm.AuthorizeAsync(orgId.Value, _permission))
            ctx.Result = new ForbidResult();
    }

    private static int? TryGetIntRouteValue(RouteData routeData, string key)
    {
        if (routeData.Values.TryGetValue(key, out var val) &&
            int.TryParse(val?.ToString(), out var id))
            return id;
        return null;
    }
}
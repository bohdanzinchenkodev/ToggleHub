using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Infrastructure.Identity.Services;

public class WorkContext : IWorkContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorkContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // only trust cookie/Identity scheme
        if (httpContext.User.Identity?.AuthenticationType != IdentityConstants.ApplicationScheme)
            return null;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}
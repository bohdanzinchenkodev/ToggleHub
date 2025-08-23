using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

public class WorkContext : IWorkContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorkContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (int.TryParse(userIdClaim, out var userId))
            return userId;
            
        return null;
    }
}
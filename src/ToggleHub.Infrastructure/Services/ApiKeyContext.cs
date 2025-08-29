using Microsoft.AspNetCore.Http;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.Infrastructure.Services;

public class ApiKeyContext : IApiKeyContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiKeyContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetCurrentOrgId() =>
        TryParseClaim("OrgId");

    public int? GetCurrentProjectId() =>
        TryParseClaim("ProjectId");

    public int? GetCurrentEnvironmentId() =>
        TryParseClaim("EnvironmentId");

    private int? TryParseClaim(string type)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.AuthenticationType != AuthConstants.AuthSchemes.ApiKey)
            return null;
        
        var value = httpContext?.User?.FindFirst(type)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
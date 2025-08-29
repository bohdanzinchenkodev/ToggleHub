using Microsoft.AspNetCore.Http;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

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
        var value = _httpContextAccessor.HttpContext?.User?.FindFirst(type)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.Services;

namespace ToggleHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register all services
        services.AddScoped<OrganizationService>();
        services.AddScoped<UserService>();
        services.AddScoped<OrgMemberService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<EnvironmentService>();
        services.AddScoped<ApiKeyService>();
        services.AddScoped<FlagService>();
        services.AddScoped<RuleService>();
        services.AddScoped<AuditLogService>();

        return services;
    }
}
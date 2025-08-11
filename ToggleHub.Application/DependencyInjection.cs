using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.Services;
using ToggleHub.Application.Validators;

namespace ToggleHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<OrganizationValidator>();

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
        
        // Register SlugGenerator
        services.AddScoped<SlugGenerator>();

        return services;
    }
}
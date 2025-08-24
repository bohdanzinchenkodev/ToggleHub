using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;


namespace ToggleHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<IApplicationMaker>();

        // Register all services
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IEnvironmentService, EnvironmentService>();
        services.AddScoped<IFlagService, FlagService>();
        services.AddScoped<IWorkContext, WorkContext>();
        services.AddScoped<IOrganizationPermissionService, OrganizationPermissionService>();
        
        // Register SlugGenerator
        services.AddScoped<ISlugGenerator, SlugGenerator>();

        return services;
    }
}
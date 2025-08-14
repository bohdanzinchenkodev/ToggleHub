using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Application.Validators;

namespace ToggleHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<IApplicationMaker>();

        // Register all services
        services.AddScoped<IOrganizationService, OrganizationService>();
        
        
        // Register SlugGenerator
        services.AddScoped<ISlugGenerator, SlugGenerator>();

        return services;
    }
}
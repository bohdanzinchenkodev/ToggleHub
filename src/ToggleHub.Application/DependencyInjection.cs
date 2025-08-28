using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.EventHandlers;
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
        services.AddScoped<IApiKeyService, ApiKeyService>();

        services.AddScoped<IFlagEvaluationService, FlagEvaluationService>();
        services.AddScoped<IBucketingService, Sha256BucketingService>();
        services.AddScoped<IConditionEvaluator, ConditionEvaluator>();
        services.AddScoped<IApiKeyGenerator, ApiKeyGenerator>();
        
        // Register SlugGenerator
        services.AddScoped<ISlugGenerator, SlugGenerator>();
        // Register Event Handlers
        services.RegisterEventHandlers();

        return services;
    }
    
    public static IServiceCollection RegisterEventHandlers(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        var eventHandlerInterfaceType = typeof(IConsumer<>);
        var eventHandlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
            .Where(ti => ti.Interface.IsGenericType && ti.Interface.GetGenericTypeDefinition() == eventHandlerInterfaceType)
            .Distinct();

        foreach (var handlerTypeTuple in eventHandlerTypes)
        {
            services.AddScoped(handlerTypeTuple.Interface, handlerTypeTuple.Type);
        }

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}
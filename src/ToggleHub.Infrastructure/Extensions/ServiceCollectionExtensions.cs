using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ToggleHub.Application;
using ToggleHub.Application.EventHandlers;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Cache;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Email;
using ToggleHub.Infrastructure.Repositories;
using ToggleHub.Infrastructure.Services;
using ToggleHub.Infrastructure.Settings;

namespace ToggleHub.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ToggleHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrgMemberRepository, OrgMemberRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IFlagRepository, FlagRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IRuleSetRepository, RuleSetRepository>();
        services.AddScoped<IRuleConditionRepository, RuleConditionRepository>();
        services.AddScoped<IRuleConditionItemRepository, RuleConditionItemRepository>();
        services.AddScoped<IOrganizationInviteRepository, OrganizationInviteRepository>();
        
        // Cache
        services.AddMemoryCache();
        services.AddScoped<ICacheManager, InMemoryCacheManager>();
        services.AddSingleton<ICacheKeyRegistry, InMemoryCacheKeyRegistry>();
        services.AddScoped<ICacheKeyFormatter, CacheKeyFormatter>();
        services.AddScoped<ICacheKeyFactory, CacheKeyFactory>();
        services.AddScoped<IFlagEvaluationCacheKeyFactory, FlagEvaluationCacheKeyFactory>();
        
        
        //Email
        services.AddScoped<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();
        services.AddScoped<IEmailSender, SendGridEmailSender>();
        services.AddScoped<IUrlBuilder, UrlBuilder>();
        
        services.AddScoped<IApiKeyContext, ApiKeyContext>();
        
        // Register settings
        services.Configure<SendGridSettings>(configuration.GetSection("SendGrid"));
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<SendGridSettings>>().Value);
            
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrls"));
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<ApplicationUrlSettings>>().Value);
        
        services.Configure<CacheSettings>(configuration.GetSection("Cache"));
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<CacheSettings>>().Value);

        // in memory event publisher
        services.RegisterEventHandlers();

        return services;
    }
    private static IServiceCollection RegisterEventHandlers(this IServiceCollection services)
    {
        var assembly = typeof(IApplicationMaker).Assembly;
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
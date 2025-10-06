using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ToggleHub.Application;
using ToggleHub.Application.EventHandlers;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Cache;
using ToggleHub.Infrastructure.Constants;
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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsHistoryTable(DbConstants.MigrationHistoryTable, DbConstants.AppSchemeName)));

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
        
        var mailSettings = configuration.GetSection("Mail").Get<MailSettings>();
        switch (mailSettings)
        {
            case { Provider: MailProviders.SendGrid }:
                services.AddScoped<IEmailSender, SendGridEmailSender>();
                
                services.AddSingleton(mailSettings);
                break;
            case { Provider: MailProviders.Mailpit }:
                services.AddScoped<IEmailSender, MailpitEmailSender>();
                break;
            default:
                throw new InvalidOperationException("Invalid mail provider configuration.");
        }
        
        services.AddScoped<IApiKeyContext, ApiKeyContext>();
        
        // Register settings
            
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrls"));
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<ApplicationUrlSettings>>().Value);
        
        services.Configure<CacheSettings>(configuration.GetSection("Cache"));
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<CacheSettings>>().Value);
        
        services.Configure<CacheSettings>(configuration.GetSection("Mailpit"));
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<MailpitSettings>>().Value);
        
        
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<MailpitSettings>>().Value);
        services.Configure<MailpitSettings>(configuration.GetSection(MailProviders.Mailpit));
        
        services.Configure<SendGridSettings>(configuration.GetSection(MailProviders.SendGrid));
        services.AddSingleton(registeredServices =>
            registeredServices.GetRequiredService<IOptions<SendGridSettings>>().Value);

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
    
    public static async Task ApplyInfrastructureMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToggleHubDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ISluggedRepository, SluggedRepository>();
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
        services.AddScoped<ICacheKeyRegistry, InMemoryCacheKeyRegistry>();
        services.AddScoped<ICacheKeyFormatter, CacheKeyFormatter>();
        services.AddScoped<IRepositoryCacheKeyFactory, RepositoryCacheKeyFactory>();
        
        services.AddScoped<IFlagEvaluationCacheKeyFactory, FlagEvaluationCacheKeyFactory>();
        services.AddScoped<IFlagEvaluationCacheManager, FlagEvaluationCacheManager>();

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

        return services;
    }
}
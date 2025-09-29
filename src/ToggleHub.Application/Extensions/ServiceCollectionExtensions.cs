using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.EventHandlers;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Application.Validators;

namespace ToggleHub.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<IApplicationMaker>(
            includeInternalTypes: true,
            filter: result => !typeof(IIgnoreValidator).IsAssignableFrom(result.ValidatorType)
        );

        // Register all services
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IEnvironmentService, EnvironmentService>();
        services.AddScoped<IFlagService, FlagService>();
        services.AddScoped<IOrganizationPermissionService, OrganizationPermissionService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IOrgMemberService, OrgMemberService>();
        services.AddScoped<IOrganizationInviteService, OrganizationInviteService>();

        services.AddScoped<IFlagEvaluationService, FlagEvaluationService>();
        services.AddScoped<IBucketingService, Sha256BucketingService>();
        services.AddScoped<IConditionEvaluator, ConditionEvaluator>();
        services.AddScoped<IApiKeyGenerator, ApiKeyGenerator>();
        
        services.AddScoped<IInvitationEmailWorkflowService, InvitationEmailWorkflowService>();
        services.AddScoped<IPasswordResetEmailWorkflowService, PasswordResetEmailWorkflowService>();
        
        // Register SlugGenerator
        services.AddScoped<ISlugGenerator, SlugGenerator>();

        return services;
    }

 
}
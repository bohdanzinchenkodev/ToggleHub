using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Identity.Data;
using ToggleHub.Infrastructure.Identity.Entities;
using ToggleHub.Infrastructure.Identity.Services;

namespace ToggleHub.Infrastructure.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppIdentity(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ToggleHubIdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
        
        services.AddIdentity<AppUser, IdentityRole<int>>()
            .AddRoles<IdentityRole<int>>()   
            .AddEntityFrameworkStores<ToggleHubIdentityDbContext>()
            .AddDefaultTokenProviders();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        services.AddScoped<IIdentityService, IdentityService>();
        
        return services;
    }
}
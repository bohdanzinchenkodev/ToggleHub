using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Identity.Constants;
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
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsHistoryTable(DbConstants.MigrationHistoryTable, DbConstants.IdentitySchemeName)));
        
        services.AddIdentity<AppUser, AppRole>()
            .AddRoles<AppRole>()   
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

        services.AddHttpContextAccessor();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkContext, WorkContext>();
        
        
        return services;
    }
    public static async Task ApplyIdentityMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToggleHubIdentityDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Infrastructure.Identity.Data;
using ToggleHub.Infrastructure.Identity.Entities;

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
            .AddEntityFrameworkStores<ToggleHubIdentityDbContext>()
            .AddDefaultTokenProviders();
        
        return services;
    }
}
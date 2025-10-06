using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Constants;
using ToggleHub.Infrastructure.Identity.Constants;
using ToggleHub.Infrastructure.Identity.Entities;

namespace ToggleHub.Infrastructure.Identity.Data;

public class ToggleHubIdentityDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    public ToggleHubIdentityDbContext(DbContextOptions<ToggleHubIdentityDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(DbConstants.IdentitySchemeName);
        base.OnModelCreating(builder);
        builder.Entity<AppUser>(b =>
        {
            // Each User can have many UserClaims
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
        });
        builder.Entity<AppUserRole>(b =>
        {
            b.HasOne(r => r.Role)
                .WithMany()
                .HasForeignKey(r => r.RoleId)
                .IsRequired();
            
            b.HasOne(r => r.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(r => r.UserId)
                .IsRequired();
        });
        
        AppRole[] roles =
        [
            new()
            {
                Id = 1,
                Name = UserConstants.UserRoles.Admin,
                NormalizedName = UserConstants.UserRoles.Admin.ToUpper(),
                ConcurrencyStamp = "7dd5f15b-e5b2-459c-8bf2-827ce0947bdd"
            },
            new()
            {
                Id = 2,
                Name = UserConstants.UserRoles.User,
                NormalizedName = UserConstants.UserRoles.User.ToUpper(),
                ConcurrencyStamp = "ca3ee765-3024-4ebd-bf86-1f7764751b88"
            }
        ];

        builder.Entity<AppRole>().HasData(roles);
    }
}
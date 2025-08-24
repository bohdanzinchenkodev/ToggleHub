using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToggleHub.Infrastructure.Identity.Entities;

namespace ToggleHub.Infrastructure.Identity.Data;

public class ToggleHubIdentityDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    public ToggleHubIdentityDbContext(DbContextOptions<ToggleHubIdentityDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
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
    }
}
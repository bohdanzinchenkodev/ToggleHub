using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToggleHub.Infrastructure.Identity.Entities;

namespace ToggleHub.Infrastructure.Identity.Data;

public class ToggleHubIdentityDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public ToggleHubIdentityDbContext(DbContextOptions<ToggleHubIdentityDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
    }
}
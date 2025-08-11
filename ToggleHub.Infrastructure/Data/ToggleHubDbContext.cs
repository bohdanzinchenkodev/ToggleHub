using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Infrastructure.Data;

public class ToggleHubDbContext : DbContext
{
    public ToggleHubDbContext(DbContextOptions<ToggleHubDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<OrgMember> OrgMembers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Environment> Environments { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Flag> Flags { get; set; }
    public DbSet<Rule> Rules { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Organization
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure OrgMember
        modelBuilder.Entity<OrgMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrgId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrgId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.OrgId, e.UserId }).IsUnique();
        });

        // Configure Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrgId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrgId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.OrgId, e.Slug }).IsUnique();
        });

        // Configure Environment
        modelBuilder.Entity<Environment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.ProjectId, e.Name }).IsUnique();
        });

        // Configure ApiKey
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrgId).IsRequired();
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.EnvironmentId).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Prefix).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Hash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Scopes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrgId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Environment)
                  .WithMany()
                  .HasForeignKey(e => e.EnvironmentId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.Prefix).IsUnique();
        });

        // Configure Flag
        modelBuilder.Entity<Flag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrgId).IsRequired();
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.EnvironmentId).IsRequired();
            entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Enabled).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrgId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Environment)
                  .WithMany()
                  .HasForeignKey(e => e.EnvironmentId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.EnvironmentId, e.Key }).IsUnique();
        });

        // Configure Rule
        modelBuilder.Entity<Rule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FlagId).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.ConditionsJson).HasMaxLength(4000);
            entity.Property(e => e.Value).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Flag)
                  .WithMany()
                  .HasForeignKey(e => e.FlagId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.FlagId, e.Priority }).IsUnique();
        });

        // Configure AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrgId).IsRequired();
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.EnvironmentId).IsRequired();
            entity.Property(e => e.Actor).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TargetType).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TargetId).IsRequired();
            entity.Property(e => e.DiffJson).HasMaxLength(8000);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrgId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Environment)
                  .WithMany()
                  .HasForeignKey(e => e.EnvironmentId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.OrgId, e.CreatedAt });
        });
    }
}

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
    public DbSet<OrgMember> OrgMembers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Environment> Environments { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Flag> Flags { get; set; }
    public DbSet<RuleSet> RuleSets { get; set; }
    public DbSet<RuleCondition> RuleConditions { get; set; }
    public DbSet<RuleConditionItem> RuleConditionItems { get; set; }
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
        

        // Configure OrgMember
        modelBuilder.Entity<OrgMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrganizationId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
            
        });

        // Configure Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrganizationId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Environment
        modelBuilder.Entity<Environment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
           
            
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ApiKey
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrganizationId).IsRequired();
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.EnvironmentId).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Prefix).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Hash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Scopes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Environment)
                  .WithMany()
                  .HasForeignKey(e => e.EnvironmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Flag
        modelBuilder.Entity<Flag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.EnvironmentId).IsRequired();
            entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Enabled).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
                  
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Environment)
                  .WithMany()
                  .HasForeignKey(e => e.EnvironmentId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(e => e.RuleSets)
                    .WithOne(e => e.Flag)
                    .HasForeignKey(e => e.FlagId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Rule
        modelBuilder.Entity<RuleSet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FlagId).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.Percentage).IsRequired().HasDefaultValue(100);
            entity.Property(e => e.BucketingSeed).IsRequired();
            
            entity.HasOne(e => e.Flag)
                  .WithMany(x => x.RuleSets)
                  .HasForeignKey(e => e.FlagId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(e => e.Conditions)
                    .WithOne(e => e.RuleSet)
                    .HasForeignKey(e => e.RuleSetId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure RuleCondition
        modelBuilder.Entity<RuleCondition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RuleSetId).IsRequired();
            entity.Property(e => e.Field).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FieldType).IsRequired();
            entity.Property(e => e.Operator).IsRequired();
            entity.Property(e => e.ValueString).HasMaxLength(1000);
            
            entity.HasOne(e => e.RuleSet)
                  .WithMany(rs => rs.Conditions)
                  .HasForeignKey(e => e.RuleSetId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasMany(e => e.Items)
                    .WithOne()
                    .HasForeignKey(i => i.RuleConditionId)
                    .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure RuleConditionItem
        modelBuilder.Entity<RuleConditionItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RuleConditionId).IsRequired();
            entity.Property(e => e.ValueString).HasMaxLength(1000);
            
            entity.HasOne(e => e.RuleCondition)
                  .WithMany(rc => rc.Items)
                  .HasForeignKey(e => e.RuleConditionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrganizationId).IsRequired();
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
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Environment)
                  .WithMany()
                  .HasForeignKey(e => e.EnvironmentId)
                  .OnDelete(DeleteBehavior.Cascade);
            
        });
    }
}

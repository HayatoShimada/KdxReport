using Microsoft.EntityFrameworkCore;
using KdxReport.Models;

namespace KdxReport.Data;

public class KdxReportDbContext : DbContext
{
    public KdxReportDbContext(DbContextOptions<KdxReportDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleUser> RoleUsers { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<TripReport> TripReports { get; set; }
    public DbSet<Models.Thread> Threads { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<ReadStatus> ReadStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Role configurations
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName).IsUnique();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // RoleUser configurations
        modelBuilder.Entity<RoleUser>(entity =>
        {
            entity.HasIndex(e => new { e.RoleId, e.UserId }).IsUnique();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Equipment configurations
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // TripReport configurations
        modelBuilder.Entity<TripReport>(entity =>
        {
            entity.Property(e => e.ApprovalStatus)
                .HasDefaultValue("pending");

            entity.HasOne(t => t.Approver)
                .WithMany(u => u.ApprovedTripReports)
                .HasForeignKey(t => t.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Thread configurations
        modelBuilder.Entity<Models.Thread>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Comment configurations
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Attachment configurations
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ReadStatus configurations
        modelBuilder.Entity<ReadStatus>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.TripReportId }).IsUnique();

            entity.Property(e => e.IsRead)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default roles with fixed datetime
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 1,
                RoleName = "Admin",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Role
            {
                RoleId = 2,
                RoleName = "User",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
            if (updatedAtProperty != null)
            {
                updatedAtProperty.CurrentValue = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added)
            {
                var createdAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                if (createdAtProperty != null)
                {
                    createdAtProperty.CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}

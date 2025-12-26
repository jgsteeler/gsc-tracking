using Microsoft.EntityFrameworkCore;
using GscTracking.Core.Entities;
using GscTracking.Core.Enums;

namespace GscTracking.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customer { get; set; }
    public DbSet<Job> Job { get; set; }
    public DbSet<JobUpdate> JobUpdate { get; set; }
    public DbSet<Expense> Expense { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema only for PostgreSQL
        if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            modelBuilder.HasDefaultSchema("public");
        }

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            
            // Global query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EquipmentType).IsRequired().HasMaxLength(200);
            entity.Property(e => e.EquipmentModel).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DateReceived).IsRequired();
            // Use precision instead of ColumnType for cross-database compatibility
            entity.Property(e => e.EstimateAmount).HasPrecision(18, 2);
            entity.Property(e => e.ActualAmount).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            // Foreign key relationship
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Jobs)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Global query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<JobUpdate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UpdateText).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.Job)
                .WithMany(j => j.JobUpdates)
                .HasForeignKey(e => e.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.ReceiptReference).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.Job)
                .WithMany(j => j.Expenses)
                .HasForeignKey(e => e.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

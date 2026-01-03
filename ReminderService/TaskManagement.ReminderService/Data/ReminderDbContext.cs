using Microsoft.EntityFrameworkCore;
using TaskManagement.ReminderService.Models;

namespace TaskManagement.ReminderService.Data;

public class ReminderDbContext : DbContext
{
    public ReminderDbContext(DbContextOptions<ReminderDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.ToTable("Tasks");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Telephone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.DueDate).HasDatabaseName("IX_Tasks_DueDate");
            entity.HasIndex(e => e.LastReminderSentAt).HasDatabaseName("IX_Tasks_LastReminderSentAt");
        });
    }
}

using CapstoneGenerator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneGenerator.API.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
    }

    public DbSet<AnalyticsEntry> AnalyticsEntries { get; set; }
    public DbSet<Review> Reviews { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    modelBuilder.Entity<AnalyticsEntry>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Course).HasMaxLength(255);
        entity.Property(e => e.Difficulty).HasMaxLength(50);
        entity.Property(e => e.GeneratedAt).IsRequired();
        entity.Property(e => e.Success).IsRequired();
    });

    modelBuilder.Entity<Review>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).HasMaxLength(50);
        entity.Property(e => e.Message).HasMaxLength(500).IsRequired();
        entity.Property(e => e.Rating).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
    });
}
}

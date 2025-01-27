using Microsoft.EntityFrameworkCore;
using OutboxPattern.WebAPI.Models;

namespace OutboxPattern.WebAPI.Context;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderOutbox> OrderOutboxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(builder =>
        {
            builder.Property(p => p.ProductName).HasColumnType("varchar(50)");
            builder.Property(p => p.CustomerEmail).HasColumnType("varchar(350)");
            builder.Property(p => p.Price).HasColumnType("money");
        });
    }
}
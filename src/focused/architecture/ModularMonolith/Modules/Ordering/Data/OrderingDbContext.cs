using Microsoft.EntityFrameworkCore;
using Ordering.Domain;

namespace Ordering.Data;

// Internal — owned exclusively by the Ordering module, separate from
// CatalogDbContext. Each module persists its own data.
internal class OrderingDbContext(DbContextOptions<OrderingDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Lines)
            .WithOne()
            .HasForeignKey(l => l.OrderId);
    }
}

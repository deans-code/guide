using Microsoft.EntityFrameworkCore;

namespace CQRSDemo.Common;

// ── Write-side entities (normalised) ──────────────────────────────────────
// Commands mutate these tables. They are never read by queries.

public enum OrderStatus { Pending, Confirmed, Cancelled }

public class Order
{
    public Guid Id { get; set; }
    public required string CustomerName { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime PlacedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
}

public class OrderLine
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
}

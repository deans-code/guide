using Microsoft.EntityFrameworkCore;

namespace CQRSDemo.Common;

// ── Read-side model (denormalised) ────────────────────────────────────────
// Queries read only from this table. It is never written to by commands —
// only by projection handlers that react to domain events.
//
// The shape here is driven by what the UI needs, not by the write schema.
// In production this would often be a separate database (Redis, Cosmos,
// Elasticsearch) optimised for the specific query patterns.

public class OrderSummary
{
    public Guid Id { get; set; }
    public required string CustomerName { get; set; }
    public required string Status { get; set; }
    public int LineCount { get; set; }
    public decimal Total { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

public class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public DbSet<OrderSummary> OrderSummaries => Set<OrderSummary>();
}

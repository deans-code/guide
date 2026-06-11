using MediatR;
using CQRSDemo.Common;
using CQRSDemo.Write;

namespace CQRSDemo.Read;

// ── Projections ───────────────────────────────────────────────────────────
// Each handler listens for a domain event from the write side and updates
// the read model accordingly. They only ever touch ReadDbContext.
//
// Consistency model: in-process MediatR makes this synchronous within the
// same request, so the read model is immediately consistent here. In a
// distributed system (separate services, message broker) there would be a
// short propagation delay — queries would need to tolerate stale reads.

public class OnOrderPlaced(ReadDbContext db) : INotificationHandler<OrderPlacedEvent>
{
    public async Task Handle(OrderPlacedEvent e, CancellationToken ct)
    {
        db.OrderSummaries.Add(new OrderSummary
        {
            Id           = e.OrderId,
            CustomerName = e.CustomerName,
            Status       = "Pending",
            LineCount    = e.LineCount,
            Total        = e.Total,
            PlacedAt     = e.PlacedAt
        });
        await db.SaveChangesAsync(ct);
    }
}

public class OnOrderConfirmed(ReadDbContext db) : INotificationHandler<OrderConfirmedEvent>
{
    public async Task Handle(OrderConfirmedEvent e, CancellationToken ct)
    {
        var summary = await db.OrderSummaries.FindAsync([e.OrderId], ct);
        if (summary is null) return;

        summary.Status      = "Confirmed";
        summary.ConfirmedAt = e.ConfirmedAt;
        await db.SaveChangesAsync(ct);
    }
}

public class OnOrderCancelled(ReadDbContext db) : INotificationHandler<OrderCancelledEvent>
{
    public async Task Handle(OrderCancelledEvent e, CancellationToken ct)
    {
        var summary = await db.OrderSummaries.FindAsync([e.OrderId], ct);
        if (summary is null) return;

        summary.Status = "Cancelled";
        await db.SaveChangesAsync(ct);
    }
}

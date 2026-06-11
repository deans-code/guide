using MediatR;
using CQRSDemo.Common;

namespace CQRSDemo.Write;

// ── Domain event ──────────────────────────────────────────────────────────
// Carries only the data the read side needs — not the write-side entity.
// This decouples the projection from the write model's internal structure.

public record OrderPlacedEvent(
    Guid OrderId,
    string CustomerName,
    int LineCount,
    decimal Total,
    DateTime PlacedAt) : INotification;

// ── Command ───────────────────────────────────────────────────────────────

public record OrderLineRequest(string ProductName, int Quantity, decimal UnitPrice);

public record PlaceOrderCommand(string CustomerName, List<OrderLineRequest> Lines)
    : IRequest<Guid>;

// ── Handler ───────────────────────────────────────────────────────────────

public class PlaceOrderHandler(WriteDbContext db, IMediator mediator)
    : IRequestHandler<PlaceOrderCommand, Guid>
{
    public async Task<Guid> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var order = new Order
        {
            Id           = Guid.NewGuid(),
            CustomerName = cmd.CustomerName,
            PlacedAt     = DateTime.UtcNow,
            Lines        = cmd.Lines.Select(l => new OrderLine
            {
                Id          = Guid.NewGuid(),
                ProductName = l.ProductName,
                Quantity    = l.Quantity,
                UnitPrice   = l.UnitPrice
            }).ToList()
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        // Publishing the event causes the projection handler to sync the read model.
        // In-process here; in a distributed system this would go via a message broker.
        await mediator.Publish(new OrderPlacedEvent(
            order.Id,
            order.CustomerName,
            order.Lines.Count,
            order.Lines.Sum(l => l.Quantity * l.UnitPrice),
            order.PlacedAt), ct);

        return order.Id;
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class PlaceOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/orders", async (PlaceOrderCommand cmd, IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(cmd, ct);
            return Results.Created($"/orders/{id}", new { id });
        })
        .WithTags("Write — Commands")
        .WithSummary("Place an order (mutates write model, projects to read model via OrderPlacedEvent)");
}

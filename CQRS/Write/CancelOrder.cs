using MediatR;
using CQRSDemo.Common;

namespace CQRSDemo.Write;

// ── Domain event ──────────────────────────────────────────────────────────

public record OrderCancelledEvent(Guid OrderId) : INotification;

// ── Command ───────────────────────────────────────────────────────────────

public record CancelOrderCommand(Guid OrderId) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────

public class CancelOrderHandler(WriteDbContext db, IMediator mediator)
    : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand cmd, CancellationToken ct)
    {
        var order = await db.Orders.FindAsync([cmd.OrderId], ct);
        if (order is null || order.Status == OrderStatus.Cancelled) return false;

        order.Status = OrderStatus.Cancelled;
        await db.SaveChangesAsync(ct);

        await mediator.Publish(new OrderCancelledEvent(order.Id), ct);

        return true;
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class CancelOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/orders/{id:guid}/cancel", async (Guid id, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new CancelOrderCommand(id), ct)
                ? Results.Ok(new { message = "Order cancelled" })
                : Results.BadRequest(new { error = "Order not found or already cancelled" }))
        .WithTags("Write — Commands")
        .WithSummary("Cancel an order (projects status change to read model via OrderCancelledEvent)");
}

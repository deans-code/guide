using MediatR;
using CQRSDemo.Common;

namespace CQRSDemo.Write;

// ── Domain event ──────────────────────────────────────────────────────────

public record OrderConfirmedEvent(Guid OrderId, DateTime ConfirmedAt) : INotification;

// ── Command ───────────────────────────────────────────────────────────────

public record ConfirmOrderCommand(Guid OrderId) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────

public class ConfirmOrderHandler(WriteDbContext db, IMediator mediator)
    : IRequestHandler<ConfirmOrderCommand, bool>
{
    public async Task<bool> Handle(ConfirmOrderCommand cmd, CancellationToken ct)
    {
        var order = await db.Orders.FindAsync([cmd.OrderId], ct);
        if (order is null || order.Status != OrderStatus.Pending) return false;

        order.Status      = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await mediator.Publish(new OrderConfirmedEvent(order.Id, order.ConfirmedAt.Value), ct);

        return true;
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class ConfirmOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/orders/{id:guid}/confirm", async (Guid id, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new ConfirmOrderCommand(id), ct)
                ? Results.Ok(new { message = "Order confirmed" })
                : Results.BadRequest(new { error = "Order not found or not in Pending status" }))
        .WithTags("Write — Commands")
        .WithSummary("Confirm a pending order (projects status change to read model via OrderConfirmedEvent)");
}

using Microsoft.EntityFrameworkCore;
using OutboxDemo.Data;
using OutboxDemo.Services;

namespace OutboxDemo.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapPost("/", async (
            PlaceOrderRequest req,
            IOrderService orderService,
            CancellationToken ct) =>
        {
            var order = await orderService.PlaceOrderAsync(req.CustomerName, req.Amount, ct);
            return Results.Created($"/orders/{order.Id}", new
            {
                order.Id,
                order.CustomerName,
                order.Amount,
                order.Status,
                order.CreatedAt,
                outbox = "An 'order.placed' message was written atomically in the same transaction. " +
                         "Check /outbox to see it as Pending — it will be published within 5 seconds."
            });
        })
        .WithSummary("Place an order — writes the order row and an outbox message in one transaction");

        group.MapGet("/", async (AppDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync(ct)))
        .WithSummary("List all orders");

        group.MapGet("/{id:guid}", async (Guid id, AppDbContext db, CancellationToken ct) =>
            await db.Orders.FindAsync([id], ct) is Order order
                ? Results.Ok(order)
                : Results.NotFound())
        .WithSummary("Get a single order by ID");
    }

    public record PlaceOrderRequest(string CustomerName, decimal Amount);
}

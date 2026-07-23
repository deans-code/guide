using MassTransit;
using Microsoft.EntityFrameworkCore;
using MassTransitDemo.Data;
using MassTransitDemo.Messages;

namespace MassTransitDemo.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapPost("/", async (
            PlaceOrderRequest req,
            AppDbContext db,
            IPublishEndpoint publisher,
            CancellationToken ct) =>
        {
            var order = new Order { CustomerName = req.CustomerName, Amount = req.Amount };
            db.Orders.Add(order);
            await db.SaveChangesAsync(ct);

            // A single Publish fans out to every subscriber independently —
            // SendConfirmationEmailConsumer and UpdateInventoryConsumer both
            // receive this event without the publisher knowing they exist.
            await publisher.Publish(new OrderPlaced(order.Id, order.CustomerName, order.Amount, order.CreatedAt), ct);

            return Results.Created($"/orders/{order.Id}", new
            {
                order.Id,
                order.CustomerName,
                order.Amount,
                order.CreatedAt,
                note = "OrderPlaced published — check /orders/{id} shortly to see both consumers' activity"
            });
        })
        .WithSummary("Place an order — publishes OrderPlaced to two independent consumers");

        group.MapGet("/", async (AppDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync(ct)))
        .WithSummary("List all orders");

        group.MapGet("/{id:guid}", async (Guid id, AppDbContext db, CancellationToken ct) =>
        {
            var order = await db.Orders.FindAsync([id], ct);
            if (order is null)
                return Results.NotFound();

            var logs = await db.ConsumerLogs
                .Where(l => l.OrderId == id)
                .OrderBy(l => l.ProcessedAt)
                .ToListAsync(ct);

            return Results.Ok(new { order, consumerActivity = logs });
        })
        .WithSummary("Get an order along with every consumer's processing activity for it");
    }

    public record PlaceOrderRequest(string CustomerName, decimal Amount);
}

using Microsoft.EntityFrameworkCore;
using MassTransitDemo.Data;
using MassTransitDemo.Services;

namespace MassTransitDemo.Endpoints;

public static class ConsumerEndpoints
{
    public static void MapConsumerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/consumers").WithTags("Consumers");

        group.MapGet("/activity", async (AppDbContext db, CancellationToken ct) =>
            Results.Ok(await db.ConsumerLogs
                .OrderByDescending(l => l.ProcessedAt)
                .ToListAsync(ct)))
        .WithSummary("List every consumer's processing activity across all orders, newest first");

        // Controls how many times UpdateInventoryConsumer fails before succeeding.
        // Clamped to [0, 3] to match the 3 retries configured in Program.cs — set it
        // to 3 to watch a message exhaust its retries and fault.
        group.MapPut("/inventory/failures-before-success", (int count, FlakyInventoryService flaky) =>
        {
            flaky.FailuresBeforeSuccess = Math.Clamp(count, 0, 3);
            return Results.Ok(new
            {
                flaky.FailuresBeforeSuccess,
                note = flaky.FailuresBeforeSuccess switch
                {
                    0 => "UpdateInventoryConsumer will succeed on the first attempt",
                    3 => "UpdateInventoryConsumer will fail on every retry and the message will fault — " +
                         "MassTransit's retry policy allows 3 retries (4 attempts total)",
                    _ => $"UpdateInventoryConsumer will fail {flaky.FailuresBeforeSuccess} time(s) " +
                         "before succeeding — place an order and watch /orders/{id} fill in across attempts"
                }
            });
        })
        .WithSummary("Set how many times UpdateInventoryConsumer fails before succeeding (0-3)");
    }
}

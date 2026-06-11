using Microsoft.EntityFrameworkCore;
using OutboxDemo.Data;
using OutboxDemo.Services;

namespace OutboxDemo.Endpoints;

public static class OutboxEndpoints
{
    public static void MapOutboxEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/outbox").WithTags("Outbox");

        group.MapGet("/", async (AppDbContext db, CancellationToken ct) =>
            Results.Ok(await db.OutboxMessages
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new
                {
                    m.Id,
                    m.Type,
                    Status      = m.Status.ToString(),
                    m.AttemptCount,
                    m.CreatedAt,
                    m.ProcessedAt,
                    m.LastError,
                    m.Payload
                })
                .ToListAsync(ct)))
        .WithSummary("List all outbox messages with current status");

        group.MapGet("/summary", async (AppDbContext db, CancellationToken ct) =>
        {
            var counts = await db.OutboxMessages
                .GroupBy(m => m.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(ct);

            return Results.Ok(new
            {
                counts,
                pollIntervalSeconds = OutboxProcessor.PollInterval.TotalSeconds,
                note = "Pending messages are published within one poll interval. " +
                       "Failed = dead-lettered after 3 attempts."
            });
        })
        .WithSummary("Count of outbox messages grouped by status");

        // Manually trigger a processing batch — no need to wait for the poll interval.
        group.MapPost("/process", async (OutboxProcessor processor, CancellationToken ct) =>
        {
            var processed = await processor.ProcessBatchAsync(ct);
            return Results.Ok(new
            {
                processed,
                note = processed == 0
                    ? "No pending messages in the outbox"
                    : $"Processed {processed} message(s) — check /outbox for updated statuses"
            });
        })
        .WithSummary("Manually trigger one processing batch (bypasses the 5s poll interval)");

        // Adjust the simulated broker's reliability to observe retry and dead-letter behaviour.
        group.MapPut("/publisher/failure-rate", (double rate, IEventPublisher publisher) =>
        {
            publisher.FailureRate = Math.Clamp(rate, 0.0, 1.0);
            return Results.Ok(new
            {
                failureRate = publisher.FailureRate,
                note = publisher.FailureRate switch
                {
                    0.0 => "Publisher will succeed every time",
                    1.0 => "Publisher will always fail — messages will be dead-lettered after 3 attempts",
                    _   => $"Publisher will fail ~{publisher.FailureRate * 100:F0}% of the time — watch AttemptCount climb"
                }
            });
        })
        .WithSummary("Set the simulated broker failure rate (0.0 = always succeed, 1.0 = always fail)");

        // Requeue dead-lettered messages back to Pending so they can be retried.
        group.MapPost("/requeue-failed", async (AppDbContext db, CancellationToken ct) =>
        {
            var failed = await db.OutboxMessages
                .Where(m => m.Status == OutboxStatus.Failed)
                .ToListAsync(ct);

            foreach (var m in failed)
            {
                m.Status       = OutboxStatus.Pending;
                m.AttemptCount = 0;
                m.LastError    = null;
            }

            await db.SaveChangesAsync(ct);
            return Results.Ok(new { requeued = failed.Count });
        })
        .WithSummary("Requeue all dead-lettered (Failed) messages back to Pending");
    }
}

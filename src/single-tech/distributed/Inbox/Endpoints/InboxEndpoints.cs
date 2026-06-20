using Microsoft.EntityFrameworkCore;
using InboxDemo.Data;
using InboxDemo.Services;

namespace InboxDemo.Endpoints;

public static class InboxEndpoints
{
    public static void MapInboxEndpoints(this WebApplication app)
    {
        var inboxGroup = app.MapGroup("/inbox").WithTags("Inbox");

        inboxGroup.MapGet("/", async (AppDbContext db, CancellationToken ct) =>
            Results.Ok(await db.InboxMessages
                .OrderByDescending(m => m.ReceivedAt)
                .Select(m => new
                {
                    m.MessageId,
                    m.Type,
                    Status = m.Status.ToString(),
                    m.ReceivedAt,
                    m.ProcessedAt,
                    m.Payload
                })
                .ToListAsync(ct)))
        .WithSummary("List all inbox messages with current status");

        inboxGroup.MapPost("/process", async (InboxProcessor processor, CancellationToken ct) =>
        {
            var processed = await processor.ProcessBatchAsync(ct);
            return Results.Ok(new
            {
                processed,
                note = processed == 0
                    ? "No pending messages in the inbox"
                    : $"Processed {processed} message(s) — check /fulfillments for the results"
            });
        })
        .WithSummary("Manually trigger one processing batch (bypasses the 5s poll interval)");

        var fulfillmentsGroup = app.MapGroup("/fulfillments").WithTags("Fulfillments");

        fulfillmentsGroup.MapGet("/", async (AppDbContext db, CancellationToken ct) =>
            Results.Ok(await db.FulfillmentRecords
                .OrderByDescending(f => f.FulfilledAt)
                .ToListAsync(ct)))
        .WithSummary("List all fulfillment records created by processing inbox messages");
    }
}

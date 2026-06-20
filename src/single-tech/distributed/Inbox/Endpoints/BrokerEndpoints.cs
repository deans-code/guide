using System.Text.Json;
using InboxDemo.Data;

namespace InboxDemo.Endpoints;

public static class BrokerEndpoints
{
    public static void MapBrokerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/broker").WithTags("Broker");

        group.MapPost("/deliver", async (
            DeliverMessageRequest req,
            AppDbContext db,
            CancellationToken ct) =>
        {
            var existing = await db.InboxMessages.FindAsync([req.MessageId], ct);
            if (existing is not null)
            {
                return Results.Ok(new
                {
                    messageId = req.MessageId,
                    status = "duplicate",
                    note = "This message ID was already received. The inbox discarded the duplicate — the order will not be fulfilled twice."
                });
            }

            db.InboxMessages.Add(new InboxMessage
            {
                MessageId = req.MessageId,
                Type = req.Type,
                Payload = JsonSerializer.Serialize(req.Payload)
            });
            await db.SaveChangesAsync(ct);

            return Results.Accepted(null, new
            {
                messageId = req.MessageId,
                status = "pending",
                note = "Message stored in inbox. It will be processed within 5 seconds. Check /inbox to observe status."
            });
        })
        .WithSummary("Simulate a message arriving from the broker — re-send the same MessageId to see deduplication");
    }

    public record DeliverMessageRequest(Guid MessageId, string Type, JsonElement Payload);
}

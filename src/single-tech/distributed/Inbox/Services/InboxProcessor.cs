using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using InboxDemo.Data;

namespace InboxDemo.Services;

public class InboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<InboxProcessor> logger) : BackgroundService
{
    private const int BatchSize = 10;
    public static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "[InboxProcessor] Started — polling every {Interval}s",
            PollInterval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "[InboxProcessor] Unexpected error during batch");
            }

            try { await Task.Delay(PollInterval, stoppingToken); }
            catch (OperationCanceledException) { break; }
        }
    }

    // Public so the /inbox/process endpoint can trigger a batch on demand.
    public async Task<int> ProcessBatchAsync(CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var messages = await db.InboxMessages
            .Where(m => m.Status == InboxStatus.Pending)
            .OrderBy(m => m.ReceivedAt)
            .Take(BatchSize)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                var fulfillment = Handle(message);
                db.FulfillmentRecords.Add(fulfillment);

                message.Status = InboxStatus.Processed;
                message.ProcessedAt = DateTime.UtcNow;

                logger.LogInformation(
                    "[InboxProcessor] Processed {Type}  id={MessageId}  → fulfillment {FulfillmentId}",
                    message.Type, message.MessageId, fulfillment.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[InboxProcessor] Failed to process {Type}  id={MessageId}",
                    message.Type, message.MessageId);
            }
        }

        if (messages.Count > 0)
            await db.SaveChangesAsync(ct);

        return messages.Count;
    }

    private static FulfillmentRecord Handle(InboxMessage message)
    {
        var payload = JsonSerializer.Deserialize<OrderPlacedPayload>(message.Payload)
            ?? throw new InvalidOperationException($"Invalid payload for message {message.MessageId}");

        return new FulfillmentRecord
        {
            OrderId = payload.OrderId,
            CustomerName = payload.CustomerName,
            Amount = payload.Amount,
            InboxMessageId = message.MessageId
        };
    }
}

internal record OrderPlacedPayload(Guid OrderId, string CustomerName, decimal Amount);

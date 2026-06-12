using Microsoft.EntityFrameworkCore;
using OutboxDemo.Data;

namespace OutboxDemo.Services;

// Runs as a background service, polling the outbox table and forwarding
// messages to the broker. Registered as a singleton so endpoints can call
// ProcessBatchAsync directly for demo purposes.
public class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    IEventPublisher publisher,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private const int BatchSize  = 10;
    private const int MaxAttempts = 3;
    public static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "[OutboxProcessor] Started — polling every {Interval}s, max {Max} attempts per message",
            PollInterval.TotalSeconds, MaxAttempts);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "[OutboxProcessor] Unexpected error during batch");
            }

            try { await Task.Delay(PollInterval, stoppingToken); }
            catch (OperationCanceledException) { break; }
        }
    }

    // Public so the /outbox/process endpoint can trigger a batch on demand.
    public async Task<int> ProcessBatchAsync(CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Fetch pending messages oldest-first, skipping ones that have exhausted retries.
        //
        // NOTE: In a multi-instance deployment you would use SELECT FOR UPDATE SKIP LOCKED
        // (Postgres) or application-level leases to prevent two instances picking up the
        // same message. SQLite is single-writer so this is safe here.
        var messages = await db.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Pending && m.AttemptCount < MaxAttempts)
            .OrderBy(m => m.CreatedAt)
            .Take(BatchSize)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            message.AttemptCount++;
            try
            {
                await publisher.PublishAsync(message, ct);

                message.Status      = OutboxStatus.Processed;
                message.ProcessedAt = DateTime.UtcNow;

                logger.LogInformation(
                    "[OutboxProcessor] Processed {Type}  id={Id}  attempt={Attempt}",
                    message.Type, message.Id, message.AttemptCount);
            }
            catch (Exception ex)
            {
                message.LastError = ex.Message;

                if (message.AttemptCount >= MaxAttempts)
                {
                    message.Status = OutboxStatus.Failed;
                    logger.LogError(
                        "[OutboxProcessor] Dead-lettered {Type}  id={Id}  after {Max} attempts: {Error}",
                        message.Type, message.Id, MaxAttempts, ex.Message);
                }
                else
                {
                    logger.LogWarning(
                        "[OutboxProcessor] Retry {Attempt}/{Max} failed for {Type}  id={Id}: {Error}",
                        message.AttemptCount, MaxAttempts, message.Type, message.Id, ex.Message);
                }
            }
        }

        if (messages.Count > 0)
            await db.SaveChangesAsync(ct);

        return messages.Count;
    }
}

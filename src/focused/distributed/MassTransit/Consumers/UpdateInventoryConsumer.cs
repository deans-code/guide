using MassTransit;
using MassTransitDemo.Data;
using MassTransitDemo.Messages;
using MassTransitDemo.Services;

namespace MassTransitDemo.Consumers;

// The second independent subscriber to OrderPlaced. Deliberately flaky —
// see FlakyInventoryService — to exercise the retry middleware configured
// for this consumer in Program.cs (UseMessageRetry). MassTransit redelivers
// the same message in-process; context.GetRetryAttempt() reports which try
// this is, with no attempt-counting code of our own.
public class UpdateInventoryConsumer(
    AppDbContext db,
    FlakyInventoryService flaky,
    ILogger<UpdateInventoryConsumer> logger) : IConsumer<OrderPlaced>
{
    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var message = context.Message;
        var attempt = context.GetRetryAttempt() + 1;

        if (attempt <= flaky.FailuresBeforeSuccess)
        {
            db.ConsumerLogs.Add(new ConsumerLog
            {
                OrderId = message.OrderId,
                ConsumerName = nameof(UpdateInventoryConsumer),
                Attempt = attempt,
                Succeeded = false,
                Detail = "Simulated transient inventory failure"
            });
            await db.SaveChangesAsync(context.CancellationToken);

            logger.LogWarning(
                "[UpdateInventoryConsumer] Attempt {Attempt} failed for order {OrderId} — MassTransit will retry",
                attempt, message.OrderId);

            throw new InvalidOperationException(
                $"Simulated transient inventory failure (attempt {attempt})");
        }

        db.ConsumerLogs.Add(new ConsumerLog
        {
            OrderId = message.OrderId,
            ConsumerName = nameof(UpdateInventoryConsumer),
            Attempt = attempt,
            Succeeded = true,
            Detail = $"Inventory updated for order {message.OrderId}"
        });
        await db.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation(
            "[UpdateInventoryConsumer] Inventory updated for order {OrderId} on attempt {Attempt}",
            message.OrderId, attempt);
    }
}

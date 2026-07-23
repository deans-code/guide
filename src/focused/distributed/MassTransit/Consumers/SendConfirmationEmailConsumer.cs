using MassTransit;
using MassTransitDemo.Data;
using MassTransitDemo.Messages;

namespace MassTransitDemo.Consumers;

// One of two independent subscribers to OrderPlaced. Neither consumer knows
// the other exists — MassTransit delivers the same event to both.
public class SendConfirmationEmailConsumer(AppDbContext db, ILogger<SendConfirmationEmailConsumer> logger)
    : IConsumer<OrderPlaced>
{
    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var message = context.Message;

        db.ConsumerLogs.Add(new ConsumerLog
        {
            OrderId = message.OrderId,
            ConsumerName = nameof(SendConfirmationEmailConsumer),
            Attempt = context.GetRetryAttempt() + 1,
            Succeeded = true,
            Detail = $"Confirmation email sent to {message.CustomerName} for order {message.OrderId}"
        });
        await db.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation(
            "[SendConfirmationEmailConsumer] Emailed {Customer} about order {OrderId}",
            message.CustomerName, message.OrderId);
    }
}

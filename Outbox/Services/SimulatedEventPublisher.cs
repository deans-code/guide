using OutboxDemo.Data;

namespace OutboxDemo.Services;

// Simulates publishing to a message broker (RabbitMQ, Azure Service Bus, etc.).
// FailureRate lets you test the retry/dead-letter behaviour without a real broker.
public class SimulatedEventPublisher(ILogger<SimulatedEventPublisher> logger) : IEventPublisher
{
    public double FailureRate { get; set; } = 0.0;

    public async Task PublishAsync(OutboxMessage message, CancellationToken ct = default)
    {
        await Task.Delay(100, ct); // simulate network round-trip

        if (Random.Shared.NextDouble() < FailureRate)
            throw new InvalidOperationException(
                $"Broker unavailable while publishing '{message.Type}' ({message.Id:N})");

        logger.LogInformation(
            "[Publisher] Published {Type} → broker  id={Id}  attempt={Attempt}",
            message.Type, message.Id, message.AttemptCount);
    }
}

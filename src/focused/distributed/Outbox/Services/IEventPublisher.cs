using OutboxDemo.Data;

namespace OutboxDemo.Services;

public interface IEventPublisher
{
    Task PublishAsync(OutboxMessage message, CancellationToken ct = default);
    double FailureRate { get; set; }
}

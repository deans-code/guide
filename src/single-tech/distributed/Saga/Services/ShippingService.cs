namespace SagaDemo.Services;

public class ShippingService(ServiceSettings settings)
{
    public Task ScheduleAsync(string orderId, string productId, int quantity)
    {
        if (Random.Shared.NextDouble() < settings.ShippingFailureRate)
            throw new InvalidOperationException($"No courier available for order '{orderId}'.");

        return Task.CompletedTask;
    }

    public Task CancelAsync(string orderId)
    {
        return Task.CompletedTask;
    }
}

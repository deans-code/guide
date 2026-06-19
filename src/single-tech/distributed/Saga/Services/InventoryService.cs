namespace SagaDemo.Services;

public class InventoryService(ServiceSettings settings)
{
    public Task ReserveAsync(string productId, int quantity)
    {
        if (Random.Shared.NextDouble() < settings.InventoryFailureRate)
            throw new InvalidOperationException($"Insufficient stock for product '{productId}'.");

        return Task.CompletedTask;
    }

    public Task ReleaseAsync(string productId, int quantity)
    {
        return Task.CompletedTask;
    }
}

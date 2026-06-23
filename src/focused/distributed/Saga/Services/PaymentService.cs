namespace SagaDemo.Services;

public class PaymentService(ServiceSettings settings)
{
    public Task ChargeAsync(string orderId, decimal amount)
    {
        if (Random.Shared.NextDouble() < settings.PaymentFailureRate)
            throw new InvalidOperationException($"Payment declined for order '{orderId}'.");

        return Task.CompletedTask;
    }

    public Task RefundAsync(string orderId)
    {
        return Task.CompletedTask;
    }
}

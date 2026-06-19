namespace SagaDemo.Services;

public class ServiceSettings
{
    public double InventoryFailureRate { get; set; } = 0.0;
    public double PaymentFailureRate { get; set; } = 0.0;
    public double ShippingFailureRate { get; set; } = 0.0;
}

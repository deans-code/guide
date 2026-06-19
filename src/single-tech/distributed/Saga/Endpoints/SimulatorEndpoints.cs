using SagaDemo.Services;

namespace SagaDemo.Endpoints;

public static class SimulatorEndpoints
{
    public static void MapSimulatorEndpoints(this WebApplication app)
    {
        app.MapGet("/simulator", (ServiceSettings settings) => Results.Ok(settings))
        .WithName("GetSimulatorSettings")
        .WithSummary("Get the current failure rates for each simulated service");

        app.MapPatch("/simulator", (UpdateFailureRatesRequest request, ServiceSettings settings) =>
        {
            if (request.InventoryFailureRate is { } inv) settings.InventoryFailureRate = Math.Clamp(inv, 0.0, 1.0);
            if (request.PaymentFailureRate is { } pay) settings.PaymentFailureRate = Math.Clamp(pay, 0.0, 1.0);
            if (request.ShippingFailureRate is { } ship) settings.ShippingFailureRate = Math.Clamp(ship, 0.0, 1.0);
            return Results.Ok(settings);
        })
        .WithName("UpdateFailureRates")
        .WithSummary("Set failure rates for simulated services (0.0 = never fail, 1.0 = always fail)");
    }
}

public record UpdateFailureRatesRequest(double? InventoryFailureRate, double? PaymentFailureRate, double? ShippingFailureRate);

using Dapr;
using Dapr.Client;
using DaprDemo.Data;

namespace DaprDemo.Endpoints;

public static class NotificationEndpoints
{
    private const string StoreName = "statestore";

    public static void MapNotificationEndpoints(this WebApplication app)
    {
        // The [Topic] attribute is picked up by app.MapSubscribeHandler() in Program.cs, which
        // generates the /dapr/subscribe endpoint the sidecar calls on startup to discover this route.
        app.MapPost("/events/order-placed", [Topic("pubsub", "order-placed")] async (OrderPlacedEvent evt, DaprClient dapr, ILogger<Program> logger) =>
        {
            logger.LogInformation("Received order-placed event for order {OrderId}", evt.OrderId);

            var notification = new Notification(evt.OrderId, $"Order {evt.OrderId} for {evt.Quantity}x {evt.Product} confirmed.", DateTimeOffset.UtcNow);
            await dapr.SaveStateAsync(StoreName, $"notification:{evt.OrderId}", notification);

            return Results.Ok();
        });

        app.MapGet("/notifications/{orderId}", async (string orderId, DaprClient dapr) =>
        {
            var notification = await dapr.GetStateAsync<Notification>(StoreName, $"notification:{orderId}");
            return notification is null ? Results.NotFound() : Results.Ok(notification);
        });
    }
}

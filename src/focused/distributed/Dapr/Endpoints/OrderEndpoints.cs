using Dapr.Client;
using DaprDemo.Data;

namespace DaprDemo.Endpoints;

public static class OrderEndpoints
{
    private const string StoreName = "statestore";
    private const string PubSubName = "pubsub";
    private const string Topic = "order-placed";

    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/orders", async (CreateOrderRequest request, DaprClient dapr) =>
        {
            var order = new Order(Guid.NewGuid().ToString(), request.Product, request.Quantity, DateTimeOffset.UtcNow);

            // The app never talks to Redis directly — both calls go to the local Dapr sidecar,
            // which is the only thing that knows the state store and broker are backed by Redis.
            await dapr.SaveStateAsync(StoreName, order.Id, order);
            await dapr.PublishEventAsync(PubSubName, Topic, new OrderPlacedEvent(order.Id, order.Product, order.Quantity));

            return Results.Created($"/orders/{order.Id}", order);
        });

        app.MapGet("/orders/{id}", async (string id, DaprClient dapr) =>
        {
            var order = await dapr.GetStateAsync<Order>(StoreName, id);
            return order is null ? Results.NotFound() : Results.Ok(order);
        });
    }
}

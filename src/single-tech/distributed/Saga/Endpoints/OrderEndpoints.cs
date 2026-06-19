using Microsoft.EntityFrameworkCore;
using SagaDemo.Data;
using SagaDemo.Saga;

namespace SagaDemo.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/orders", async (PlaceOrderRequest request, OrderFulfillmentSaga saga, CancellationToken ct) =>
        {
            var instance = await saga.ExecuteAsync(request, ct);
            return Results.Ok(instance);
        })
        .WithName("PlaceOrder")
        .WithSummary("Start an order fulfillment saga (ReserveInventory → ProcessPayment → ScheduleShipment)");

        app.MapGet("/orders/{id:guid}", async (Guid id, AppDbContext db, CancellationToken ct) =>
        {
            var instance = await db.Sagas
                .Include(s => s.Steps)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

            return instance is null ? Results.NotFound() : Results.Ok(instance);
        })
        .WithName("GetOrder")
        .WithSummary("Get the current state and step history of a saga");

        app.MapGet("/orders", async (AppDbContext db, CancellationToken ct) =>
            await db.Sagas.Include(s => s.Steps).OrderByDescending(s => s.StartedAt).ToListAsync(ct))
        .WithName("ListOrders")
        .WithSummary("List all saga instances");
    }
}

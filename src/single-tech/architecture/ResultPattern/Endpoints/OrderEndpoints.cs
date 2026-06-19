using ErrorOr;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("/orders", async (OrderService service) =>
            Results.Ok(await service.GetAllOrdersAsync()));

        app.MapGet("/orders/{id:int}", async (int id, OrderService service) =>
        {
            var result = await service.GetOrderAsync(id);
            return result.Match(
                order => Results.Ok(order),
                errors => errors.ToProblemResult());
        });

        app.MapPost("/orders", async (PlaceOrderRequest request, OrderService service) =>
        {
            var result = await service.PlaceOrderAsync(request.ProductId, request.Quantity);
            return result.Match(
                order => Results.Created($"/orders/{order.Id}", order),
                errors => errors.ToProblemResult());
        });

        app.MapPost("/orders/{id:int}/cancel", async (int id, OrderService service) =>
        {
            var result = await service.CancelOrderAsync(id);
            return result.Match(
                order => Results.Ok(order),
                errors => errors.ToProblemResult());
        });

        app.MapGet("/products", async (AppDbContext db) =>
            Results.Ok(await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .ToListAsync(db.Products)));
    }
}

public record PlaceOrderRequest(int ProductId, int Quantity);

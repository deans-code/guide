using Dapper;
using DapperDemo.Data;

namespace DapperDemo.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("/orders/{id:int}", (int id, IDbConnectionFactory factory) =>
        {
            using var connection = factory.Create();

            // Multi-mapping: one JOIN query hydrates a full Order graph with its Items,
            // instead of the N+1 that "load the order, then load its items" would cause.
            // splitOn tells Dapper where one object's columns end and the next begins —
            // here, at each column literally named "Id".
            Order? order = null;

            connection.Query<Order, OrderItem, Order>("""
                SELECT o.Id, o.CustomerName, o.PlacedAtUtc,
                       i.Id, i.OrderId, i.ProductId, p.Name AS ProductName, i.Quantity, i.UnitPrice
                FROM Orders o
                JOIN OrderItems i ON i.OrderId = o.Id
                JOIN Products p ON p.Id = i.ProductId
                WHERE o.Id = @Id
                """,
                (o, item) =>
                {
                    order ??= o;
                    order.Items.Add(item);
                    return order;
                },
                new { Id = id },
                splitOn: "Id");

            return order is null ? Results.NotFound() : Results.Ok(order);
        })
        .WithName("GetOrder")
        .WithSummary("Get an order with its line items via Dapper multi-mapping — one query, no N+1");
    }
}

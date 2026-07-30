using Dapper;
using DapperDemo.Data;

namespace DapperDemo.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/products", (IDbConnectionFactory factory) =>
        {
            using var connection = factory.Create();
            return connection.Query<Product>("SELECT * FROM Products ORDER BY Name");
        })
        .WithName("ListProducts")
        .WithSummary("List all products");

        app.MapGet("/products/{id:int}", (int id, IDbConnectionFactory factory) =>
        {
            using var connection = factory.Create();
            var product = connection.QuerySingleOrDefault<Product>(
                "SELECT * FROM Products WHERE Id = @Id", new { Id = id });
            return product is null ? Results.NotFound() : Results.Ok(product);
        })
        .WithName("GetProduct")
        .WithSummary("Get a single product by ID");

        app.MapPost("/products", (CreateProductRequest req, IDbConnectionFactory factory) =>
        {
            using var connection = factory.Create();

            // Passing req directly as the parameter object works because Dapper reflects
            // its public properties (Name, Price, StockQuantity) to bind @Name/@Price/@StockQuantity —
            // parameterized, so there's no SQL injection risk from user input here.
            var id = connection.ExecuteScalar<long>("""
                INSERT INTO Products (Name, Price, StockQuantity)
                VALUES (@Name, @Price, @StockQuantity)
                RETURNING Id;
                """, req);

            return Results.Created($"/products/{id}", new { id });
        })
        .WithName("CreateProduct")
        .WithSummary("Create a product with a parameterized INSERT");

        app.MapPatch("/products/{id:int}/stock", (int id, AdjustStockRequest req, IDbConnectionFactory factory) =>
        {
            using var connection = factory.Create();
            var rowsAffected = connection.Execute(
                "UPDATE Products SET StockQuantity = StockQuantity + @Delta WHERE Id = @Id",
                new { Id = id, req.Delta });

            return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
        })
        .WithName("AdjustStock")
        .WithSummary("Adjust stock by a positive or negative delta — Execute() returns the affected row count");
    }
}

public record CreateProductRequest(string Name, decimal Price, int StockQuantity);
public record AdjustStockRequest(int Delta);

using Dapper;
using DapperDemo.Data;

namespace DapperDemo.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        app.MapGet("/reports/top-products", (IDbConnectionFactory factory) =>
        {
            using var connection = factory.Create();

            // A reporting query like this — aggregation, join, computed column — is exactly
            // where hand-written SQL stays readable long after the equivalent LINQ
            // expression would need escape hatches to translate cleanly.
            return connection.Query<TopProductRow>("""
                SELECT p.Id AS ProductId,
                       p.Name AS ProductName,
                       SUM(i.Quantity) AS UnitsSold,
                       SUM(i.Quantity * i.UnitPrice) AS Revenue
                FROM OrderItems i
                JOIN Products p ON p.Id = i.ProductId
                GROUP BY p.Id, p.Name
                ORDER BY Revenue DESC
                """);
        })
        .WithName("TopProducts")
        .WithSummary("Revenue and units sold per product — a GROUP BY report best expressed as raw SQL");
    }
}

// A settable-property class, not a record: Dapper materializes POCOs by matching column
// names to property setters and coerces numeric types along the way (SQLite returns
// SUM(Quantity) as Int64, for instance). A positional record instead requires a
// constructor whose parameter types match the reader's column types exactly, which
// this query's aggregate columns don't.
public class TopProductRow
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
}

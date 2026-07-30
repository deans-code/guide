using System.Data;
using Dapper;

namespace DapperDemo.Data;

public static class DbInitializer
{
    public static void Initialize(IDbConnection connection)
    {
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                StockQuantity INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerName TEXT NOT NULL,
                PlacedAtUtc TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS OrderItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL REFERENCES Orders(Id),
                ProductId INTEGER NOT NULL REFERENCES Products(Id),
                Quantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL
            );
            """);
        // REAL (not DECIMAL/NUMERIC) affinity matters here: SQLite's NUMERIC affinity
        // silently stores whole-number values like 349.00 as INTEGER while fractional
        // values like 89.99 stay REAL. Dapper's per-column deserializer is compiled from
        // the first row's CLR type and throws on a later row with a different one — REAL
        // affinity forces every value in the column to floating point, so that never happens.

        var productCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Products");
        if (productCount > 0)
            return;

        connection.Execute("""
            INSERT INTO Products (Name, Price, StockQuantity) VALUES
                ('Mechanical Keyboard', 89.99, 42),
                ('Wireless Mouse', 24.99, 120),
                ('USB-C Hub', 34.50, 75),
                ('4K Monitor', 349.00, 18);
            """);

        connection.Execute("""
            INSERT INTO Orders (CustomerName, PlacedAtUtc) VALUES
                ('Ada Lovelace', '2026-07-28T10:00:00Z'),
                ('Grace Hopper', '2026-07-29T14:30:00Z');
            """);

        connection.Execute("""
            INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES
                (1, 1, 1, 89.99),
                (1, 2, 2, 24.99),
                (2, 3, 1, 34.50),
                (2, 4, 1, 349.00),
                (2, 2, 3, 24.99);
            """);
    }
}

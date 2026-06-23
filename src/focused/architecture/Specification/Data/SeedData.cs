public static class SeedData
{
    public static void Seed(AppDbContext db)
    {
        if (db.Products.Any()) return;

        db.Products.AddRange(
            new Product { Name = "Wireless Headphones", Category = "Electronics", Price = 79.99m, StockQuantity = 42, IsFeatured = true, IsActive = true },
            new Product { Name = "Mechanical Keyboard", Category = "Electronics", Price = 149.99m, StockQuantity = 15, IsFeatured = true, IsActive = true },
            new Product { Name = "USB-C Hub", Category = "Electronics", Price = 34.99m, StockQuantity = 0, IsFeatured = false, IsActive = true },
            new Product { Name = "Laptop Stand", Category = "Electronics", Price = 29.99m, StockQuantity = 8, IsFeatured = false, IsActive = true },
            new Product { Name = "Running Shoes", Category = "Clothing", Price = 89.99m, StockQuantity = 30, IsFeatured = true, IsActive = true },
            new Product { Name = "Winter Jacket", Category = "Clothing", Price = 199.99m, StockQuantity = 5, IsFeatured = false, IsActive = true },
            new Product { Name = "Yoga Mat", Category = "Clothing", Price = 24.99m, StockQuantity = 0, IsFeatured = false, IsActive = true },
            new Product { Name = "Clean Code", Category = "Books", Price = 39.99m, StockQuantity = 20, IsFeatured = true, IsActive = true },
            new Product { Name = "The Pragmatic Programmer", Category = "Books", Price = 44.99m, StockQuantity = 12, IsFeatured = true, IsActive = true },
            new Product { Name = "Design Patterns", Category = "Books", Price = 49.99m, StockQuantity = 3, IsFeatured = false, IsActive = true },
            new Product { Name = "Discontinued Widget", Category = "Electronics", Price = 9.99m, StockQuantity = 100, IsFeatured = false, IsActive = false }
        );

        db.SaveChanges();
    }
}

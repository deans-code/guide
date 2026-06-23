public static class SeedData
{
    public static void Seed(AppDbContext db)
    {
        if (db.Products.Any()) return;

        db.Products.AddRange(
            new Product { Name = "Wireless Mouse", Price = 29.99m, Stock = 10 },
            new Product { Name = "Mechanical Keyboard", Price = 89.99m, Stock = 3 },
            new Product { Name = "USB-C Hub", Price = 49.99m, Stock = 0 }
        );

        db.SaveChanges();
    }
}

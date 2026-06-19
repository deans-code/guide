public class ActiveSpecification() : BaseSpecification<Product>(p => p.IsActive);

public class InStockSpecification() : BaseSpecification<Product>(p => p.StockQuantity > 0);

public class FeaturedSpecification() : BaseSpecification<Product>(p => p.IsFeatured);

public class CategorySpecification(string category) : BaseSpecification<Product>(p => p.Category == category);

public class PriceRangeSpecification(decimal min, decimal max)
    : BaseSpecification<Product>(p => p.Price >= min && p.Price <= max);

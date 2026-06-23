using ErrorOr;

public static class ProductErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Product.NotFound", $"Product {id} was not found.");

    public static Error InsufficientStock(string name, int available) =>
        Error.Conflict("Product.InsufficientStock", $"'{name}' only has {available} unit(s) in stock.");

    public static readonly Error InvalidQuantity =
        Error.Validation("Order.InvalidQuantity", "Quantity must be greater than zero.");
}

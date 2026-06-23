using ErrorOr;

public static class OrderErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Order.NotFound", $"Order {id} was not found.");

    public static Error AlreadyCancelled(int id) =>
        Error.Conflict("Order.AlreadyCancelled", $"Order {id} has already been cancelled.");

    public static Error AlreadyShipped(int id) =>
        Error.Conflict("Order.AlreadyShipped", $"Order {id} has already shipped and cannot be cancelled.");
}

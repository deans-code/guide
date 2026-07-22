namespace DaprDemo.Data;

public record Order(string Id, string Product, int Quantity, DateTimeOffset PlacedAt);

public record CreateOrderRequest(string Product, int Quantity);

public record OrderPlacedEvent(string OrderId, string Product, int Quantity);

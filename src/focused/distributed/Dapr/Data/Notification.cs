namespace DaprDemo.Data;

public record Notification(string OrderId, string Message, DateTimeOffset ReceivedAt);

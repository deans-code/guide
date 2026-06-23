namespace IdempotencyDemo.Data;

public class IdempotencyRecord
{
    public required string Key { get; set; }
    public required string RequestHash { get; set; }
    public required string ResponseBody { get; set; }
    public int StatusCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
}

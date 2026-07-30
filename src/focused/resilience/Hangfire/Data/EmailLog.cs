namespace HangfireDemo.Data;

public class EmailLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Subject { get; set; }
    public int AttemptsTaken { get; set; }
    public DateTime SentAtUtc { get; set; }
}

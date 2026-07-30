using System.Collections.Concurrent;
using HangfireDemo.Data;

namespace HangfireDemo.Services;

public class EmailService(AppDbContext db, ILogger<EmailService> logger) : IEmailService
{
    private static readonly ConcurrentDictionary<int, int> WelcomeAttempts = new();
    private const int FailUntilAttempt = 3;

    // Hangfire retries this job automatically on unhandled exceptions — no hand-rolled
    // retry loop in application code. The first two attempts for any given user
    // deliberately throw to simulate a flaky downstream mail provider; the third succeeds.
    public async Task SendWelcomeEmailAsync(int userId)
    {
        var attempt = WelcomeAttempts.AddOrUpdate(userId, 1, (_, n) => n + 1);
        logger.LogInformation("Sending welcome email to user {UserId}, attempt {Attempt}", userId, attempt);

        if (attempt < FailUntilAttempt)
            throw new InvalidOperationException(
                $"Simulated mail provider failure on attempt {attempt} for user {userId}");

        db.EmailLogs.Add(new EmailLog
        {
            UserId = userId,
            Subject = "Welcome!",
            AttemptsTaken = attempt,
            SentAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        WelcomeAttempts.TryRemove(userId, out _);
    }

    public async Task SendReminderAsync(int userId, string message)
    {
        logger.LogInformation("Sending reminder to user {UserId}: {Message}", userId, message);

        db.EmailLogs.Add(new EmailLog
        {
            UserId = userId,
            Subject = $"Reminder: {message}",
            AttemptsTaken = 1,
            SentAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}

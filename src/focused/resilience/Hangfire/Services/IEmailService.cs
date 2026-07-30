using Hangfire;

namespace HangfireDemo.Services;

public interface IEmailService
{
    // Enqueue<IEmailService>(...) serializes the job against this interface method, so
    // Hangfire's filter pipeline reads attributes from here — not from the implementation.
    [AutomaticRetry(Attempts = 5, DelaysInSeconds = [1, 3, 5, 8])]
    Task SendWelcomeEmailAsync(int userId);
    Task SendReminderAsync(int userId, string message);
}

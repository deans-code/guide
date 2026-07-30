using Hangfire;
using HangfireDemo.Services;

namespace HangfireDemo.Endpoints;

public static class JobEndpoints
{
    public static void MapJobEndpoints(this WebApplication app)
    {
        app.MapPost("/jobs/welcome-email/{userId:int}", (int userId, IBackgroundJobClient jobs) =>
        {
            var jobId = jobs.Enqueue<IEmailService>(s => s.SendWelcomeEmailAsync(userId));
            return Results.Accepted($"/jobs/{jobId}", new { jobId, status = "Enqueued" });
        })
        .WithName("EnqueueWelcomeEmail")
        .WithSummary("Fire-and-forget a welcome email job — fails twice before succeeding so you can watch Hangfire retry it");

        app.MapPost("/jobs/reminder/{userId:int}", (
            int userId,
            string message,
            int delaySeconds,
            IBackgroundJobClient jobs) =>
        {
            var jobId = jobs.Schedule<IEmailService>(
                s => s.SendReminderAsync(userId, message),
                TimeSpan.FromSeconds(delaySeconds));
            return Results.Accepted($"/jobs/{jobId}", new { jobId, status = "Scheduled", runsInSeconds = delaySeconds });
        })
        .WithName("ScheduleReminder")
        .WithSummary("Schedule a reminder email to run after a delay, instead of immediately");

        app.MapGet("/jobs/{jobId}", (string jobId, JobStorage storage) =>
        {
            using var connection = storage.GetConnection();
            var data = connection.GetJobData(jobId);

            return data is null
                ? Results.NotFound()
                : Results.Ok(new
                {
                    jobId,
                    state = data.State,
                    createdAt = data.CreatedAt,
                    job = data.Job?.ToString()
                });
        })
        .WithName("GetJobStatus")
        .WithSummary("Inspect a job's current state — Scheduled, Enqueued, Processing, Succeeded, or Failed");
    }
}

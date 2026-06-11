using Polly;
using Polly.Registry;
using PollyDemo.Services;

namespace PollyDemo.Endpoints;

public static class RetryEndpoints
{
    public static void MapRetryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/retry").WithTags("Retry");

        // Fixed-delay retry: waits a constant 200ms between attempts.
        group.MapGet("/fixed", async (
            ResiliencePipelineProvider<string> pipelines,
            IUnreliableService svc,
            double failureRate = 0.7) =>
        {
            svc.Reset();
            var pipeline = pipelines.GetPipeline("retry-fixed");
            try
            {
                var result = await pipeline.ExecuteAsync(ct => svc.CallAsync(failureRate, ct));
                return Results.Ok(new { strategy = "retry-fixed (constant 200ms)", result, totalCalls = svc.CallCount });
            }
            catch (Exception ex)
            {
                return Results.Ok(new
                {
                    strategy = "retry-fixed (constant 200ms)",
                    error = $"All retries exhausted: {ex.Message}",
                    totalCalls = svc.CallCount
                });
            }
        })
        .WithSummary("Retry with constant 200ms delay (max 3 attempts)");

        // Exponential backoff retry: 300ms → 600ms → 1200ms → 2400ms (+ jitter).
        group.MapGet("/exponential", async (
            ResiliencePipelineProvider<string> pipelines,
            IUnreliableService svc,
            double failureRate = 0.7) =>
        {
            svc.Reset();
            var pipeline = pipelines.GetPipeline("retry-exponential");
            try
            {
                var result = await pipeline.ExecuteAsync(ct => svc.CallAsync(failureRate, ct));
                return Results.Ok(new { strategy = "retry-exponential (backoff + jitter)", result, totalCalls = svc.CallCount });
            }
            catch (Exception ex)
            {
                return Results.Ok(new
                {
                    strategy = "retry-exponential (backoff + jitter)",
                    error = $"All retries exhausted: {ex.Message}",
                    totalCalls = svc.CallCount
                });
            }
        })
        .WithSummary("Retry with exponential backoff + jitter (max 4 attempts, base 300ms)");
    }
}

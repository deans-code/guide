using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Timeout;
using PollyDemo.Services;

namespace PollyDemo.Endpoints;

public static class CombinedEndpoints
{
    public static void MapCombinedEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/combined").WithTags("Combined Pipeline");

        // Full resilience pipeline (outermost → innermost):
        //   1. Total timeout (15s)   — budget for the whole operation including retries
        //   2. Retry (3 attempts, exponential backoff + jitter)
        //   3. Circuit breaker       — prevents hammering a broken dependency
        //   4. Per-attempt timeout (2s) — each single attempt is capped
        group.MapGet("/call", async (
            ResiliencePipelineProvider<string> pipelines,
            IUnreliableService svc,
            double failureRate = 0.7) =>
        {
            svc.Reset();
            var pipeline = pipelines.GetPipeline("combined");
            try
            {
                var result = await pipeline.ExecuteAsync(ct => svc.CallAsync(failureRate, ct));
                return Results.Ok(new
                {
                    pipeline = "timeout(15s) → retry(3x, exp) → circuit-breaker → timeout(2s/attempt)",
                    result,
                    totalCalls = svc.CallCount
                });
            }
            catch (BrokenCircuitException)
            {
                return Results.Ok(new
                {
                    pipeline = "timeout(15s) → retry(3x, exp) → circuit-breaker → timeout(2s/attempt)",
                    error = "Circuit is open — dependency is unavailable",
                    totalCalls = svc.CallCount
                });
            }
            catch (TimeoutRejectedException)
            {
                return Results.Ok(new
                {
                    pipeline = "timeout(15s) → retry(3x, exp) → circuit-breaker → timeout(2s/attempt)",
                    error = "Total operation budget (15s) exceeded",
                    totalCalls = svc.CallCount
                });
            }
            catch (Exception ex)
            {
                return Results.Ok(new
                {
                    pipeline = "timeout(15s) → retry(3x, exp) → circuit-breaker → timeout(2s/attempt)",
                    error = $"All retries exhausted: {ex.Message}",
                    totalCalls = svc.CallCount
                });
            }
        })
        .WithSummary("Full pipeline: total-timeout → retry → circuit-breaker → per-attempt-timeout");
    }
}

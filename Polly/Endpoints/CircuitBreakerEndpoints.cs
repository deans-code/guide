using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using PollyDemo.Services;

namespace PollyDemo.Endpoints;

public static class CircuitBreakerEndpoints
{
    public static void MapCircuitBreakerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/circuit-breaker").WithTags("Circuit Breaker");

        // Hit this endpoint rapidly with a high failure rate to trip the breaker.
        // Once open, calls fail immediately with BrokenCircuitException — no waiting.
        group.MapGet("/call", async (
            ResiliencePipelineProvider<string> pipelines,
            IUnreliableService svc,
            double failureRate = 0.8) =>
        {
            var pipeline = pipelines.GetPipeline("circuit-breaker");
            try
            {
                var result = await pipeline.ExecuteAsync(ct => svc.CallAsync(failureRate, ct));
                return Results.Ok(new { result, totalCalls = svc.CallCount });
            }
            catch (BrokenCircuitException ex)
            {
                return Results.Ok(new
                {
                    error = "Circuit is OPEN — request rejected immediately (no call made)",
                    detail = ex.Message,
                    totalCalls = svc.CallCount
                });
            }
            catch (Exception ex)
            {
                return Results.Ok(new
                {
                    error = $"Call failed: {ex.Message}",
                    totalCalls = svc.CallCount
                });
            }
        })
        .WithSummary("Call through the circuit breaker (failureRate=0.8 by default — trip it fast by calling repeatedly)");

        // Shows the current circuit state: Closed, Open, or HalfOpen.
        group.MapGet("/state", (CircuitBreakerMonitor monitor) =>
        {
            var state = monitor.StateProvider.CircuitState;
            return Results.Ok(new
            {
                state = state.ToString(),
                description = state switch
                {
                    CircuitState.Closed => "Normal — all requests are flowing through",
                    CircuitState.Open => "Broken — all requests are being rejected immediately",
                    CircuitState.HalfOpen => "Recovering — one probe request is allowed through",
                    CircuitState.Isolated => "Manually isolated",
                    _ => "Unknown"
                }
            });
        })
        .WithSummary("Query the current circuit breaker state");
    }
}

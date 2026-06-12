using Polly;
using Polly.Registry;
using PollyDemo.Services;

namespace PollyDemo.Endpoints;

public static class FallbackEndpoints
{
    public static void MapFallbackEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/fallback").WithTags("Fallback");

        // The pipeline retries twice, then falls back to a cached value.
        // The caller always gets a response — never an exception.
        group.MapGet("/call", async (
            ResiliencePipelineProvider<string> pipelines,
            IUnreliableService svc,
            double failureRate = 0.95) =>
        {
            svc.Reset();
            // GetPipeline<TResult> resolves the typed ResiliencePipeline<string>
            var pipeline = pipelines.GetPipeline<string>("fallback");

            var result = await pipeline.ExecuteAsync(ct => svc.CallAsync(failureRate, ct));

            return Results.Ok(new
            {
                result,
                totalCalls = svc.CallCount,
                note = result.StartsWith("(cached")
                    ? "Fallback served — all retries failed"
                    : "Live response — service recovered"
            });
        })
        .WithSummary("Fallback to cached value after 2 retries (default failureRate=0.95)");
    }
}

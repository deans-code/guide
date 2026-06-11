using Polly;
using Polly.Registry;
using Polly.Timeout;
using PollyDemo.Services;

namespace PollyDemo.Endpoints;

public static class TimeoutEndpoints
{
    public static void MapTimeoutEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/timeout").WithTags("Timeout");

        // The pipeline cancels the operation after 2 seconds.
        // Use delayMs > 2000 to trigger the timeout.
        group.MapGet("/call", async (
            ResiliencePipelineProvider<string> pipelines,
            IUnreliableService svc,
            int delayMs = 3000) =>
        {
            var pipeline = pipelines.GetPipeline("timeout");
            try
            {
                var result = await pipeline.ExecuteAsync(ct => svc.SlowCallAsync(delayMs, ct));
                return Results.Ok(new
                {
                    result,
                    note = $"Completed within the 2s timeout (delay was {delayMs}ms)"
                });
            }
            catch (TimeoutRejectedException)
            {
                return Results.Ok(new
                {
                    error = "Operation timed out after 2 seconds",
                    note = $"The call was cancelled mid-flight after {delayMs}ms delay"
                });
            }
        })
        .WithSummary("Demonstrates a 2-second timeout (default delayMs=3000 will time out)");
    }
}

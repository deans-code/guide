using Polly;
using Polly.Registry;
using PollyDemo.Services;

namespace PollyDemo.Endpoints;

public static class HedgingEndpoints
{
    public static void MapHedgingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/hedging").WithTags("Hedging");

        // If the primary attempt doesn't respond within 500ms, a parallel
        // (hedged) copy is fired. The first successful response wins.
        // Use a high delayMs value to see hedging fire.
        group.MapGet("/call", async (
            ResiliencePipelineProvider<string> pipelines,
            IUnreliableService svc,
            double failureRate = 0.6,
            int delayMs = 600) =>
        {
            svc.Reset();
            var pipeline = pipelines.GetPipeline<string>("hedging");

            try
            {
                var result = await pipeline.ExecuteAsync(async ct =>
                {
                    // Simulate a slow-then-maybe-failing service
                    await Task.Delay(delayMs, ct);
                    return await svc.CallAsync(failureRate, ct);
                });

                return Results.Ok(new
                {
                    result,
                    totalCalls = svc.CallCount,
                    note = $"Hedging fires a parallel attempt after 500ms. With {delayMs}ms delay, hedged copies are triggered."
                });
            }
            catch (Exception ex)
            {
                return Results.Ok(new
                {
                    error = $"All hedged attempts exhausted: {ex.Message}",
                    totalCalls = svc.CallCount
                });
            }
        })
        .WithSummary("Hedging: fires parallel copies after 500ms delay (max 3 hedged attempts)");
    }
}

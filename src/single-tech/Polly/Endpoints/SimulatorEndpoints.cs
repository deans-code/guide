namespace PollyDemo.Endpoints;

public static class SimulatorEndpoints
{
    public static void MapSimulatorEndpoints(this WebApplication app)
    {
        // Used by the HTTP client demo endpoints to simulate a flaky upstream service.
        app.MapGet("/simulate", async (double failureRate = 0.5, int delayMs = 0) =>
        {
            if (delayMs > 0)
                await Task.Delay(delayMs);

            if (Random.Shared.NextDouble() < failureRate)
                return Results.Problem(
                    title: "Simulated upstream failure",
                    statusCode: StatusCodes.Status503ServiceUnavailable);

            return Results.Ok(new
            {
                message = "ok",
                timestamp = DateTime.UtcNow
            });
        })
        .WithName("Simulate")
        .WithSummary("Simulates a flaky upstream service");
    }
}

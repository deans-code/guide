namespace PollyDemo.Endpoints;

public static class HttpClientEndpoints
{
    public static void MapHttpClientEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/http").WithTags("HttpClient Resilience");

        // Custom resilience pipeline attached to the named HttpClient.
        // The pipeline is configured in Program.cs via AddResilienceHandler.
        group.MapGet("/resilient", async (
            IHttpClientFactory factory,
            HttpContext ctx,
            double failureRate = 0.7,
            int delayMs = 0) =>
        {
            var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
            var client = factory.CreateClient("resilient-http");
            try
            {
                var response = await client.GetAsync(
                    $"{baseUrl}/simulate?failureRate={failureRate}&delayMs={delayMs}");

                return Results.Ok(new
                {
                    client = "resilient-http (timeout→retry→circuit-breaker→attempt-timeout)",
                    statusCode = (int)response.StatusCode,
                    body = await response.Content.ReadAsStringAsync()
                });
            }
            catch (Exception ex)
            {
                return Results.Ok(new
                {
                    client = "resilient-http",
                    error = ex.GetType().Name,
                    detail = ex.Message
                });
            }
        })
        .WithSummary("HttpClient with custom Polly pipeline (retry + circuit-breaker + timeouts)");

        // AddStandardResilienceHandler applies Microsoft's opinionated defaults:
        // rate-limiter → total-timeout(30s) → retry(3x, exp) → circuit-breaker → attempt-timeout(10s)
        group.MapGet("/standard", async (
            IHttpClientFactory factory,
            HttpContext ctx,
            double failureRate = 0.5,
            int delayMs = 0) =>
        {
            var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
            var client = factory.CreateClient("standard-http");
            try
            {
                var response = await client.GetAsync(
                    $"{baseUrl}/simulate?failureRate={failureRate}&delayMs={delayMs}");

                return Results.Ok(new
                {
                    client = "standard-http (AddStandardResilienceHandler defaults)",
                    statusCode = (int)response.StatusCode,
                    body = await response.Content.ReadAsStringAsync()
                });
            }
            catch (Exception ex)
            {
                return Results.Ok(new
                {
                    client = "standard-http",
                    error = ex.GetType().Name,
                    detail = ex.Message
                });
            }
        })
        .WithSummary("HttpClient using AddStandardResilienceHandler (Microsoft's opinionated defaults)");
    }
}

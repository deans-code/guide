using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Scalar.AspNetCore;
using PollyDemo;
using PollyDemo.Endpoints;
using PollyDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IUnreliableService, UnreliableService>();
builder.Services.AddPollyPipelines();

// ── HttpClient: custom resilience pipeline ─────────────────────────────────
// Composed pipeline (outer → inner):
//   total timeout (5s) → retry (3x, exponential) → circuit breaker → attempt timeout (2s)
builder.Services.AddHttpClient("resilient-http")
    .AddResilienceHandler("http-resilience", pipeline =>
    {
        pipeline
            .AddTimeout(TimeSpan.FromSeconds(5))
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(300),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500),
                OnRetry = static args =>
                {
                    Console.WriteLine(
                        $"[http-retry] attempt {args.AttemptNumber + 1}, " +
                        $"delay {args.RetryDelay.TotalMilliseconds:F0}ms, " +
                        $"status {(int?)args.Outcome.Result?.StatusCode}");
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = 0.5,
                MinimumThroughput = 5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(10),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500),
                OnOpened = static args =>
                {
                    Console.WriteLine(
                        $"[http-circuit-breaker] OPEN — {args.BreakDuration.TotalSeconds}s");
                    return default;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(2));
    });

// ── HttpClient: standard resilience (Microsoft opinionated defaults) ────────
// Adds: rate-limiter → total-timeout(30s) → retry(3x) → circuit-breaker → attempt-timeout(10s)
builder.Services.AddHttpClient("standard-http")
    .AddStandardResilienceHandler();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapRetryEndpoints();
app.MapCircuitBreakerEndpoints();
app.MapTimeoutEndpoints();
app.MapFallbackEndpoints();
app.MapHedgingEndpoints();
app.MapCombinedEndpoints();
app.MapHttpClientEndpoints();
app.MapSimulatorEndpoints();

app.Run();

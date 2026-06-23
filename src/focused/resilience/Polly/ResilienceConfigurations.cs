using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Hedging;
using Polly.Retry;
using Polly.Timeout;

namespace PollyDemo;

// Stores the circuit breaker state provider so endpoints can query current state.
public sealed class CircuitBreakerMonitor
{
    public CircuitBreakerStateProvider StateProvider { get; } = new();
}

public static class ResilienceConfigurations
{
    public static IServiceCollection AddPollyPipelines(this IServiceCollection services)
    {
        services.AddSingleton<CircuitBreakerMonitor>();

        // ── 1. Retry — fixed delay ─────────────────────────────────────────────
        services.AddResiliencePipeline("retry-fixed", builder =>
            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = static args =>
                {
                    Console.WriteLine(
                        $"[retry-fixed] attempt {args.AttemptNumber + 1}, " +
                        $"delay {args.RetryDelay.TotalMilliseconds}ms, " +
                        $"reason: {args.Outcome.Exception?.Message}");
                    return default;
                }
            }));

        // ── 2. Retry — exponential backoff with jitter ─────────────────────────
        services.AddResiliencePipeline("retry-exponential", builder =>
            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 4,
                Delay = TimeSpan.FromMilliseconds(300),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = static args =>
                {
                    Console.WriteLine(
                        $"[retry-exponential] attempt {args.AttemptNumber + 1}, " +
                        $"delay {args.RetryDelay.TotalMilliseconds:F0}ms");
                    return default;
                }
            }));

        // ── 3. Circuit breaker ─────────────────────────────────────────────────
        // Opens when ≥50% of the last 4+ requests fail within a 10-second window.
        // Stays open 5 seconds, then moves to half-open to probe recovery.
        services.AddResiliencePipeline("circuit-breaker", (builder, context) =>
        {
            var monitor = context.ServiceProvider.GetRequiredService<CircuitBreakerMonitor>();

            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                StateProvider = monitor.StateProvider,
                FailureRatio = 0.5,
                MinimumThroughput = 4,
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(5),
                OnOpened = static args =>
                {
                    Console.WriteLine(
                        $"[circuit-breaker] OPEN — breaking for {args.BreakDuration.TotalSeconds}s. " +
                        $"Reason: {args.Outcome.Exception?.Message}");
                    return default;
                },
                OnClosed = static _ =>
                {
                    Console.WriteLine("[circuit-breaker] CLOSED — normal operation resumed");
                    return default;
                },
                OnHalfOpened = static _ =>
                {
                    Console.WriteLine("[circuit-breaker] HALF-OPEN — probing with single request");
                    return default;
                }
            });
        });

        // ── 4. Timeout ────────────────────────────────────────────────────────
        services.AddResiliencePipeline("timeout", builder =>
            builder.AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(2),
                OnTimeout = static args =>
                {
                    Console.WriteLine($"[timeout] Timed out after {args.Timeout.TotalSeconds}s");
                    return default;
                }
            }));

        // ── 5. Fallback — typed string result ──────────────────────────────────
        // Outer: fallback to cached value after inner strategies are exhausted.
        // Inner: retry twice before giving up.
        services.AddResiliencePipeline<string, string>("fallback", builder =>
            builder
                .AddFallback(new FallbackStrategyOptions<string>
                {
                    FallbackAction = static _ =>
                        Outcome.FromResultAsValueTask<string>("(cached fallback value)"),
                    OnFallback = static args =>
                    {
                        Console.WriteLine(
                            $"[fallback] Serving cached result. Error: {args.Outcome.Exception?.Message}");
                        return default;
                    }
                })
                .AddRetry(new RetryStrategyOptions<string>
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromMilliseconds(100),
                    OnRetry = static args =>
                    {
                        Console.WriteLine($"[fallback/retry] attempt {args.AttemptNumber + 1}");
                        return default;
                    }
                }));

        // ── 6. Hedging — fire parallel requests, use first success ─────────────
        // If the primary call doesn't succeed within 500ms, a hedged (parallel)
        // copy is fired. Up to 3 hedged copies can run concurrently.
        services.AddResiliencePipeline<string, string>("hedging", builder =>
            builder.AddHedging(new HedgingStrategyOptions<string>
            {
                MaxHedgedAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                ActionGenerator = static args =>
                    () =>
                    {
                        Console.WriteLine($"[hedging] Starting hedged attempt {args.AttemptNumber}");
                        return args.Callback(args.ActionContext);
                    }
            }));

        // ── 7. Combined pipeline ───────────────────────────────────────────────
        // Execution order (outer → inner):
        //   total timeout (15s) → retry → circuit breaker → per-attempt timeout (2s)
        services.AddResiliencePipeline("combined", builder =>
            builder
                .AddTimeout(TimeSpan.FromSeconds(15))
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(300),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = static args =>
                    {
                        Console.WriteLine(
                            $"[combined/retry] attempt {args.AttemptNumber + 1}, " +
                            $"delay {args.RetryDelay.TotalMilliseconds:F0}ms");
                        return default;
                    }
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    MinimumThroughput = 4,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = TimeSpan.FromSeconds(10),
                    OnOpened = static args =>
                    {
                        Console.WriteLine(
                            $"[combined/circuit-breaker] OPEN — {args.BreakDuration.TotalSeconds}s");
                        return default;
                    }
                })
                .AddTimeout(TimeSpan.FromSeconds(2)));

        return services;
    }
}

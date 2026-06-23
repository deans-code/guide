# Polly

**Primary category:** Resilience
**Also relevant to:** Distributed

Demonstrates common Polly v8 resilience strategies: retry (fixed + exponential backoff), circuit breaker, timeout, fallback, hedging, and combined pipelines. Includes `HttpClient` integration via `AddResilienceHandler` and `AddStandardResilienceHandler`.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, Polly v8, Microsoft.Extensions.Http.Resilience

## Run

```bash
cd src/focused/resilience/Polly
dotnet run
```

- API: `http://localhost:5138`
- Swagger UI: `http://localhost:5138/scalar/v1`

## Use cases

**What it solves:** Transient failures in distributed systems. Network calls, database connections, and downstream HTTP services fail intermittently. Without resilience policies, a single bad response propagates into an unhandled exception or a hung request.

**When to reach for it:**
- Any outbound HTTP call to a third-party API or microservice
- Database connections subject to connection pool exhaustion or brief unavailability
- Message broker publish calls that can experience transient network errors

**Strategy guide:**

| Strategy | Use when |
|---|---|
| Retry (fixed) | Failures are random and brief — transient network blips |
| Retry (exponential + jitter) | Downstream is overloaded — back off to avoid thundering herd |
| Circuit breaker | A dependency is reliably broken — fail fast rather than queue up timeouts |
| Timeout | A call may hang indefinitely — enforce a maximum wait per attempt |
| Fallback | A stale cached response is acceptable when the live service is unavailable |
| Hedging | Tail latency matters — fire parallel requests and take the first to respond |
| Combined pipeline | Production HTTP clients — layer timeout → retry → circuit breaker together |

**Real-world fit:** Any .NET service making outbound calls. The `AddStandardResilienceHandler()` extension is a sensible starting point for most `HttpClient` registrations.

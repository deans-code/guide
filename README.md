# Cram
Demo code for technologies discussed during interviewing in 2026.

## Projects

### Polly — .NET 10 Minimal API
Demonstrates common Polly v8 resilience strategies: retry (fixed + exponential backoff), circuit breaker, timeout, fallback, hedging, and combined pipelines. Includes `HttpClient` integration via `AddResilienceHandler` and `AddStandardResilienceHandler`.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, Polly v8, Microsoft.Extensions.Http.Resilience

```bash
cd Polly
dotnet run
```

- API: `http://localhost:5138`
- Swagger UI: `http://localhost:5138/scalar/v1`

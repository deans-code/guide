# Hangfire

**Primary category:** Resilience
**Also relevant to:** Data

Demonstrates background job processing with Hangfire: fire-and-forget jobs with automatic retry, delayed/scheduled jobs, and a recurring (CRON) job. The welcome-email job deliberately fails its first two attempts to simulate a flaky mail provider — Hangfire retries it automatically via a declarative `[AutomaticRetry]` policy, with no hand-rolled attempt-counting or backoff logic in application code. Job state is persisted to SQLite, so scheduled and retrying jobs survive a process restart.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, Hangfire 1.8, Hangfire.Storage.SQLite, EF Core 10, SQLite

## Run

```bash
cd src/focused/resilience/Hangfire
dotnet run
```

- API: `http://localhost:5212`
- Swagger UI: `http://localhost:5212/scalar/v1`
- Hangfire dashboard: `http://localhost:5212/hangfire`

## Use cases

**What it solves:** Some work is too slow, too unreliable, or too unimportant to make a caller wait for — sending an email, generating a report, calling a flaky third-party API. Doing it inline ties up a request thread and means a downstream failure becomes a failed HTTP response. Hangfire moves that work out of the request path, persists it so it isn't lost on a crash or restart, and retries it automatically when it fails.

**When to reach for it:**
- Sending emails, push notifications, or webhooks after an HTTP request completes, without blocking the response
- Calling a flaky or rate-limited third-party API where "try again later" is an acceptable outcome
- Scheduled housekeeping — nightly digests, report generation, stale-data cleanup — without standing up a separate scheduler service
- Any operation you want to survive an application restart mid-flight, not just an in-process `Task.Run`

**Job types in this demo:**

| Type | API | Example here |
|---|---|---|
| Fire-and-forget | `IBackgroundJobClient.Enqueue` | `POST /jobs/welcome-email/{userId}` — runs as soon as a worker is free |
| Delayed | `IBackgroundJobClient.Schedule` | `POST /jobs/reminder/{userId}` — runs after a given delay |
| Recurring | `IRecurringJobManager.AddOrUpdate` | `digest-generation` — runs on a CRON schedule (`Cron.Minutely()` here for demo visibility; production would use `Cron.Hourly()` or `Cron.Daily()`) |

**Trade-offs to be aware of:**
- `[AutomaticRetry]` must be applied to the *interface* method used in `Enqueue<TService>(...)`, not the implementation — Hangfire serializes the job against the interface's `MethodInfo` and reads filter attributes from there
- Retries mask transient failures but still delay the outcome; a job that always throws exhausts its attempts and moves to the Failed state, it doesn't succeed
- The Hangfire dashboard has no authentication configured here — fine for a local demo, but production deployments must lock it down (`IDashboardAuthorizationFilter`)
- Job arguments are serialized (to JSON by default) and stored until processed — don't pass large payloads or sensitive data directly; pass an ID and re-fetch inside the job instead

**Relationship to other patterns:** [Polly](../Polly/README.md) retries an in-flight call inline, within the lifetime of a single request; Hangfire retries a job that has already been persisted and detached from the request, so it survives a crash or restart between attempts. [MassTransit](../../distributed/MassTransit/README.md) solves a related but distinct problem — reacting to a message from another service — whereas Hangfire schedules work the current process itself decides to defer.

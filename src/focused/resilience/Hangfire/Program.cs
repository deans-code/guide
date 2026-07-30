using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using HangfireDemo.Data;
using HangfireDemo.Endpoints;
using HangfireDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=hangfire-demo.db"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDigestService, DigestService>();

// Job state (Scheduled, Enqueued, Retrying, Succeeded, Failed) is persisted to its own
// SQLite file, separate from application data — jobs survive a process restart just
// like they would against SQL Server or Redis in production.
builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage("hangfire-jobs.db"));

builder.Services.AddHangfireServer();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

    // Registered once at startup; Hangfire persists the schedule itself, so it survives
    // restarts without re-registration. A minutely cadence keeps the demo observable —
    // production digests would typically use Cron.Hourly() or Cron.Daily().
    scope.ServiceProvider.GetRequiredService<IRecurringJobManager>().AddOrUpdate<IDigestService>(
        "digest-generation",
        s => s.GenerateDigestAsync(),
        Cron.Minutely());
}

app.MapOpenApi();
app.MapScalarApiReference();

// No auth configured — fine for a local demo, never do this in production.
app.UseHangfireDashboard("/hangfire");

app.MapJobEndpoints();
app.MapEmailEndpoints();

app.Run();

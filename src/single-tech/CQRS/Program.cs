using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using CQRSDemo.Common;

var builder = WebApplication.CreateBuilder(args);

// ── Two separate DbContexts — one per side ────────────────────────────────
// Using the same SQLite file here for demo simplicity.
// In production, these would typically be different databases optimised for
// their respective workloads (e.g. PostgreSQL for writes, Redis for reads).
// Separate database files reinforce the CQRS separation — in production
// these would be entirely different database technologies.
builder.Services.AddDbContext<WriteDbContext>(opt =>
    opt.UseSqlite("Data Source=cqrs-write.db"));

builder.Services.AddDbContext<ReadDbContext>(opt =>
    opt.UseSqlite("Data Source=cqrs-read.db"));

// MediatR discovers all IRequestHandler<,> and INotificationHandler<> in
// the assembly — commands, queries, and projection handlers are all wired up
// automatically.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<WriteDbContext>().Database.EnsureCreated();
    scope.ServiceProvider.GetRequiredService<ReadDbContext>().Database.EnsureCreated();
}

app.MapOpenApi();
app.MapScalarApiReference();

// Discover and register all IEndpoint implementations.
foreach (var endpoint in typeof(Program).Assembly.GetTypes()
    .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpoint).IsAssignableFrom(t))
    .Select(t => (IEndpoint)Activator.CreateInstance(t)!))
{
    endpoint.MapEndpoint(app);
}

app.Run();

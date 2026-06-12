using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using OutboxDemo.Data;
using OutboxDemo.Endpoints;
using OutboxDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=outbox.db"));

builder.Services.AddScoped<IOrderService, OrderService>();

// Singleton so the failure rate survives across requests and the background service
// shares the same instance as the /outbox/publisher/failure-rate endpoint.
builder.Services.AddSingleton<IEventPublisher, SimulatedEventPublisher>();

// Register as singleton so endpoints can call ProcessBatchAsync directly,
// then wire up as a hosted service using that same instance.
builder.Services.AddSingleton<OutboxProcessor>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<OutboxProcessor>());

builder.Services.AddOpenApi();

var app = builder.Build();

// Create the SQLite schema on startup (idempotent).
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapOrderEndpoints();
app.MapOutboxEndpoints();

app.Run();

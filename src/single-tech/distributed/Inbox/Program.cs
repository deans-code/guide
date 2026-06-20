using InboxDemo.Data;
using InboxDemo.Endpoints;
using InboxDemo.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=inbox.db"));

// Registered as singleton so endpoints can call ProcessBatchAsync directly,
// then wired up as a hosted service using that same instance.
builder.Services.AddSingleton<InboxProcessor>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<InboxProcessor>());

builder.Services.AddOpenApi();

var app = builder.Build();

// Create the SQLite schema on startup (idempotent).
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapBrokerEndpoints();
app.MapInboxEndpoints();

app.Run();

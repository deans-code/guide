using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using IdempotencyDemo.Data;
using IdempotencyDemo.Endpoints;
using IdempotencyDemo.Idempotency;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=idempotency.db"));

builder.Services.AddScoped<IdempotencyService>();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapPaymentEndpoints();

app.Run();

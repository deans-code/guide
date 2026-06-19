using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SagaDemo.Data;
using SagaDemo.Endpoints;
using SagaDemo.Saga;
using SagaDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=saga.db"));

// Singleton so failure rates persist across requests
builder.Services.AddSingleton<ServiceSettings>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ShippingService>();
builder.Services.AddScoped<OrderFulfillmentSaga>();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapOrderEndpoints();
app.MapSimulatorEndpoints();

app.Run();

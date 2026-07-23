using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using MassTransitDemo.Consumers;
using MassTransitDemo.Data;
using MassTransitDemo.Endpoints;
using MassTransitDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=masstransit.db"));

builder.Services.AddSingleton<FlakyInventoryService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SendConfirmationEmailConsumer>();

    // Retry only applies to this consumer's endpoint — SendConfirmationEmailConsumer
    // is unaffected. Three retries with a short fixed delay is enough to demonstrate
    // the behaviour without slowing the demo down.
    x.AddConsumer<UpdateInventoryConsumer>(cfg =>
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromMilliseconds(250))));

    // In-memory transport keeps this demo to `dotnet run` — no broker to install.
    // Swapping to RabbitMQ or Azure Service Bus in production is a change to this
    // one block; consumers and message contracts are untouched.
    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
});

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapOrderEndpoints();
app.MapConsumerEndpoints();

app.Run();

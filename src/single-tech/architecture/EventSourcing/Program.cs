using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using EventSourcingDemo.Accounts;
using EventSourcingDemo.Endpoints;
using EventSourcingDemo.EventStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=eventsourcing.db"));

builder.Services.AddScoped<EventStoreRepository>();
builder.Services.AddScoped<BankAccountService>();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapAccountEndpoints();

app.Run();

using Scalar.AspNetCore;
using DaprDemo.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddOpenApi();

var app = builder.Build();

// Unwraps the CloudEvents envelope the Dapr sidecar wraps pub/sub messages in
// before they reach the [Topic]-attributed handlers below.
app.UseCloudEvents();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapSubscribeHandler();
app.MapOrderEndpoints();
app.MapNotificationEndpoints();

app.Run();

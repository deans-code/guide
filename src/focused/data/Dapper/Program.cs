using Scalar.AspNetCore;
using DapperDemo.Data;
using DapperDemo.Endpoints;

var builder = WebApplication.CreateBuilder(args);

const string connectionString = "Data Source=dapper-demo.db";

// Dapper works directly against IDbConnection — no DbContext, no change tracking. The
// factory hands out a fresh, already-open connection per operation, which mirrors how
// Dapper is used in practice: short-lived connections around a single query or command.
builder.Services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(connectionString));

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
    using var connection = factory.Create();
    DbInitializer.Initialize(connection);
}

app.MapOpenApi();
app.MapScalarApiReference();

app.MapProductEndpoints();
app.MapOrderEndpoints();
app.MapReportEndpoints();

app.Run();

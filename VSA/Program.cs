using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VSADemo.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=vsa.db"));

// MediatR scans the assembly for all IRequestHandler<,> implementations.
// Adding a new feature slice is all that's needed — no registration here.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// FluentValidation scans for all AbstractValidator<T> implementations.
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Plug validation into the MediatR pipeline — runs before every handler.
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

// Map ValidationException → 400, everything else → 500.
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (ex is ValidationException ve)
    {
        ctx.Response.StatusCode  = StatusCodes.Status400BadRequest;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new
        {
            errors = ve.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
        });
    }
    else
    {
        ctx.Response.StatusCode  = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new { error = ex?.Message });
    }
}));

app.MapOpenApi();
app.MapScalarApiReference();

// ── Endpoint discovery ────────────────────────────────────────────────────
// Each feature slice registers its own route by implementing IEndpoint.
// Program.cs never needs to know about individual features.
foreach (var endpoint in typeof(Program).Assembly.GetTypes()
    .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpoint).IsAssignableFrom(t))
    .Select(t => (IEndpoint)Activator.CreateInstance(t)!))
{
    endpoint.MapEndpoint(app);
}

app.Run();

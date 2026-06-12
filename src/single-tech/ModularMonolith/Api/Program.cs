using Catalog;
using Common;
using Ordering;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// The host only knows modules through IModule — it never references a
// module's internal DbContexts, entities, or feature handlers directly.
// Adding a new module is a one-line change here.
IModule[] modules = [new CatalogModule(), new OrderingModule()];

foreach (var module in modules)
    module.AddModule(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

foreach (var module in modules)
{
    module.EnsureDatabaseCreated(app.Services);
    module.MapEndpoints(app);
}

app.Run();

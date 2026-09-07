using Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Integration;
using Inventory;
using Ordering;
using OrderSync.Api;
using Scalar.AspNetCore;
using Serilog;
using Shared.Infrastructure.Data;
using Shared.Infrastructure.Endpoints;
using Shared.Infrastructure.Modules;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logger) => logger
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSharedPersistence(builder.Configuration);

builder.Services.AddModules(
    builder.Configuration,
    new InventoryModule(),
    new OrderingModule(),
    new IntegrationModule(),
    new IdentityModule());

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseMigrator(builder.Configuration);
}

WebApplication app = builder.Build();

// Before the exception handler on purpose: by the time the request-logging middleware sees the
// response, the handler has already turned the exception into a 500, so the failure is logged
// once with full detail there and once as a request summary here.
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapEndpoints();

// Liveness runs no check on purpose: it answers "is the process alive", and its only sensible
// consequence is a restart. Readiness answers "can it serve traffic" - a database that is still
// coming up must take us out of rotation without restarting anything.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();

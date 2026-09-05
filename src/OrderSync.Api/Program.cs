using Identity;
using Integration;
using Inventory;
using Ordering;
using Shared.Infrastructure.Data;
using Shared.Infrastructure.Endpoints;
using Shared.Infrastructure.Modules;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

app.MapEndpoints();

app.Run();

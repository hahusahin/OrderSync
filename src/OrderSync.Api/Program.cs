using Identity;
using Integration;
using Inventory;
using Ordering;
using Shared.Infrastructure.Endpoints;
using Shared.Infrastructure.Modules;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddModules(
    builder.Configuration,
    new InventoryModule(),
    new OrderingModule(),
    new IntegrationModule(),
    new IdentityModule());

WebApplication app = builder.Build();

app.MapEndpoints();

app.Run();

using Inventory.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Data;
using Shared.Infrastructure.Modules;

namespace Inventory;

public sealed class InventoryModule : IModule
{
    public string Name => "Inventory";

    public void AddModule(IServiceCollection services, IConfiguration configuration) =>
        services.AddModuleDbContext<InventoryDbContext>(InventoryDbContext.Schema);
}

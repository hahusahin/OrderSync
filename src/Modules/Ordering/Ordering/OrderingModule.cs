using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Data;
using Shared.Infrastructure.Data;
using Shared.Infrastructure.Modules;

namespace Ordering;

public sealed class OrderingModule : IModule
{
    public string Name => "Ordering";

    public void AddModule(IServiceCollection services, IConfiguration configuration) =>
        services.AddModuleDbContext<OrderingDbContext>(OrderingDbContext.Schema);
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Modules;

namespace Ordering;

public sealed class OrderingModule : IModule
{
    public string Name => "Ordering";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
    }
}

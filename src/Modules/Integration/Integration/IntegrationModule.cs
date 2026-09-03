using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Modules;

namespace Integration;

public sealed class IntegrationModule : IModule
{
    public string Name => "Integration";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
    }
}

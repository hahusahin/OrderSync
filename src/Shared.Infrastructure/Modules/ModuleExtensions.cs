using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Endpoints;

namespace Shared.Infrastructure.Modules;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration,
        params IModule[] modules)
    {
        foreach (IModule module in modules)
        {
            services.AddSingleton(module);
            module.AddModule(services, configuration);
            services.AddEndpointsFromAssembly(module.Assembly);
        }

        return services;
    }
}

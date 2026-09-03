using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Modules;

namespace Identity;

public sealed class IdentityModule : IModule
{
    public string Name => "Identity";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
    }
}

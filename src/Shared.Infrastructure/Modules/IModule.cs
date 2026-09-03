using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Modules;

/// <summary>
/// A module's only entry point into the host. The host knows the module by this interface and
/// by nothing else.
/// </summary>
public interface IModule
{
    string Name { get; }

    Assembly Assembly => GetType().Assembly;

    void AddModule(IServiceCollection services, IConfiguration configuration);
}

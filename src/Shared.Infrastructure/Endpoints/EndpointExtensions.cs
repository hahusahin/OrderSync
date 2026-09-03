using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Infrastructure.Endpoints;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpointsFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] descriptors = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false }
                           && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(descriptors);

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        foreach (IEndpoint endpoint in app.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>())
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}

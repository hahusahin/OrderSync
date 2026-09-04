using Microsoft.AspNetCore.Routing;

namespace Shared.Infrastructure.Endpoints;

/// <summary>
/// One HTTP route, declared next to the use case it serves instead of in a shared controller.
/// Implementations are resolved from the root provider while routes are being mapped, so their
/// constructors must not take scoped services (DbContext, ISender). Those are taken as handler
/// parameters, which the framework binds from the request's own scope.
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

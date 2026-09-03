using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Endpoints;

namespace Ordering.Features.Ping;

// Placeholder: proves the module is registered and its endpoints are discovered.
// Deleted once this module has a real feature.
internal sealed class PingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/ordering/ping", () => Results.Ok(new { module = "Ordering" }))
            .WithTags("Ordering");
}

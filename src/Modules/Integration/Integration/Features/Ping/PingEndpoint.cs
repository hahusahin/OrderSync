using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Endpoints;

namespace Integration.Features.Ping;

// Placeholder: proves the module is registered and its endpoints are discovered.
// Deleted once this module has a real feature.
internal sealed class PingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/integration/ping", () => Results.Ok(new { module = "Integration" }))
            .WithTags("Integration");
}

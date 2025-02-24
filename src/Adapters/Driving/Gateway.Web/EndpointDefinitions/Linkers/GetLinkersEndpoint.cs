using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Common.Web;
using ScriptBee.Gateway.Web.EndpointDefinitions.Linkers.Contracts;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Linkers;

public class GetLinkersEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO: implement it
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/linkers", GetAllLinkers);
    }

    private static async Task<Ok<List<WebLinker>>> GetAllLinkers(
        CancellationToken cancellationToken = default
    )
    {
        await Task.CompletedTask;
        return TypedResults.Ok(new List<WebLinker> { new("test-linker-1", "Test Linker") });
    }
}

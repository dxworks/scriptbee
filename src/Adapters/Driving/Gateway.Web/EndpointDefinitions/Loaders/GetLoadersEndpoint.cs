using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Common.Web;
using ScriptBee.Gateway.Web.EndpointDefinitions.Loaders.Contracts;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Loaders;

public class GetLoadersEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO: implement it
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/loaders", GetAllLoaders);
    }

    private static async Task<Ok<List<WebLoader>>> GetAllLoaders(
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return TypedResults.Ok(new List<WebLoader>
        {
            new("test-loader-1", "Test Loader"),
        });
    }
}

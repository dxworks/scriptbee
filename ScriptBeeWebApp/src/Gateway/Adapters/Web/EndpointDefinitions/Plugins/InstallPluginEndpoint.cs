using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class InstallPluginEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        // TODO FIXIT(#87): replace hardcoded values with use cases
        // TODO FIXIT(#87): add version in request body
        app.MapPut("/api/projects/{projectId}/plugins/{pluginId}", InstallPlugin);
    }

    private static Task<Ok> InstallPlugin(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string pluginId,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(TypedResults.Ok());
    }
}

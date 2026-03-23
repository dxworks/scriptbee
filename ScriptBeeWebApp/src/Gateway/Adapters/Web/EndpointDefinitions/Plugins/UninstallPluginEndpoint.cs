using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class UninstallPluginEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        // TODO FIXIT(#87): replace hardcoded values with use cases
        app.MapDelete("/api/projects/{projectId}/plugins/{pluginId}", UninstallPlugin);
    }

    private static Task<Ok> UninstallPlugin(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string pluginId,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(TypedResults.Ok());
    }
}

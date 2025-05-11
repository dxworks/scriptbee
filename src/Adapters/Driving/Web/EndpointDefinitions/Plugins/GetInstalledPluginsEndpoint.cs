using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class GetInstalledPluginsEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        // TODO FIXIT(#87): replace hardcoded values with use cases
        app.MapGet("/api/projects/{projectId}/plugins", GetAllAvailablePlugins);
    }

    private static Task<Ok<WebInstalledPluginsResponse>> GetAllAvailablePlugins(
        HttpContext context,
        [FromRoute] string projectId,
        CancellationToken cancellationToken = default
    )
    {
        var response = new WebInstalledPluginsResponse(
            [
                new WebMarketplaceProject(
                    "plugin1",
                    "Installed Plugin 1",
                    WebMarketplaceProject.PluginType,
                    "Description for Plugin 1",
                    ["Author 1", "Author 2"],
                    [new WebPluginVersion("1.0.0")]
                ),
            ]
        );

        return Task.FromResult(TypedResults.Ok(response));
    }
}

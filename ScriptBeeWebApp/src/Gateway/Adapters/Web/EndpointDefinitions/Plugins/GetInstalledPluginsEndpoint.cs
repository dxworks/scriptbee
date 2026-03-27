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
        app.MapGet("/api/projects/{projectId}/plugins", GetInstalledPlugins);
    }

    private static Task<Ok<WebInstalledPluginsResponse>> GetInstalledPlugins(
        HttpContext context,
        [FromRoute] string projectId,
        CancellationToken cancellationToken = default
    )
    {
        var response = new WebInstalledPluginsResponse([
            new WebMarketplacePlugin(
                "plugin1",
                "Python Plugin",
                WebMarketplacePlugin.PluginType,
                "Allows running Python scripts for analysis and automation.",
                ["John Doe", "Jane Smith"],
                "1.1.0",
                "1.1.0"
            ),
            new WebMarketplacePlugin(
                "bundle1",
                "Standard Bundle",
                WebMarketplacePlugin.BundleType,
                "A collection of standard plugins for general use.",
                ["ScriptBee Team"],
                "1.0.0",
                "1.0.0"
            ),
        ]);

        return Task.FromResult(TypedResults.Ok(response));
    }
}

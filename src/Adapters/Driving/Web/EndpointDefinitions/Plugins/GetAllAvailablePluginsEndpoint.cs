using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class GetAllAvailablePluginsEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        // TODO FIXIT(#87): replace hardcoded values with use cases
        app.MapGet("/api/plugins", GetAllAvailablePlugins);
    }

    private static Task<Ok<WebAllAvailablePluginsResponse>> GetAllAvailablePlugins(
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        var response = new WebAllAvailablePluginsResponse(
            [
                new WebMarketplaceProject(
                    "plugin1",
                    "Plugin 1",
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

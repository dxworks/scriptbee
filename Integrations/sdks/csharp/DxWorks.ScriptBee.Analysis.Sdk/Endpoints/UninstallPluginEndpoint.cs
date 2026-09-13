using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Plugins;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class UninstallPluginEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/plugins/{pluginId}", UninstallPlugin)
            .WithTags("Plugins")
            .WithSummary("Uninstall a plugin")
            .WithDescription("Uninstalls a specific plugin version from the analysis service.")
            .WithName("PluginsDelete");
    }

    private static NoContent UninstallPlugin(
        HttpContext context,
        [FromRoute] string pluginId,
        [FromQuery] string version,
        IUninstallPluginUseCase uninstallPluginUseCase,
        CancellationToken cancellationToken = default
    )
    {
        uninstallPluginUseCase.UninstallPlugin(new PluginId(pluginId, new Version(version)));

        return TypedResults.NoContent();
    }
}

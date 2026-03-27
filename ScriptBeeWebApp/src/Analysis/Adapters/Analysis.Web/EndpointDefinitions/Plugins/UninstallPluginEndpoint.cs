using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Service.Plugin;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins;

public class UninstallPluginEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IUninstallPluginUseCase, UninstallPluginService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/plugins/{pluginId}", UninstallPlugin);
    }

    private static NoContent UninstallPlugin(
        HttpContext context,
        [FromRoute] string pluginId,
        [FromQuery] string version,
        IUninstallPluginUseCase uninstallPluginUseCase,
        CancellationToken cancellationToken = default
    )
    {
        uninstallPluginUseCase.UninstallPlugin(pluginId, version);

        return TypedResults.NoContent();
    }
}

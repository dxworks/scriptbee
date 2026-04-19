using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins;

public class UninstallPluginEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IUninstallPluginUseCase, UninstallPluginService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/plugins/{pluginId}", UninstallPlugin).WithTags("Plugins");
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

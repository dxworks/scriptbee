using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.UseCases.Gateway.Plugins;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class ManageGatewayPluginsEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/plugins/gateway", GetGatewayPlugins)
            .WithTags("Plugins")
            .WithSummary("Get gateway plugins")
            .WithDescription("Retrieves a list of all plugins installed at the gateway level.");
        app.MapPost("/api/plugins/gateway", InstallGatewayPlugin)
            .WithTags("Plugins")
            .WithSummary("Install gateway plugin")
            .WithDescription("Installs a new plugin at the gateway level.");
        app.MapDelete("/api/plugins/gateway/{pluginId}", UninstallGatewayPlugin)
            .WithTags("Plugins")
            .WithSummary("Uninstall gateway plugin")
            .WithDescription("Uninstalls a plugin from the gateway level.");
    }

    private static Ok<WebGatewayPluginsResponse> GetGatewayPlugins(IManagePluginsUseCase useCase)
    {
        var installedPlugins = useCase.GetInstalledPlugins();

        return TypedResults.Ok(
            new WebGatewayPluginsResponse(
                installedPlugins.Select(id => new WebInstalledPlugin(
                    id.Name,
                    id.Version.ToString()
                ))
            )
        );
    }

    private static async Task<NoContent> InstallGatewayPlugin(
        [FromBody] WebInstallGatewayPluginRequest request,
        IManagePluginsUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.Install(
            new PluginId(request.Id, new Version(request.Version)),
            cancellationToken
        );
        return TypedResults.NoContent();
    }

    private static NoContent UninstallGatewayPlugin(
        [FromRoute] string pluginId,
        [FromQuery] string version,
        IManagePluginsUseCase useCase
    )
    {
        useCase.Uninstall(new PluginId(pluginId, new Version(version)));
        return TypedResults.NoContent();
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
        app.MapGet("/api/plugins/gateway/ui/manifest", GetUiPluginsManifest)
            .WithTags("Plugins")
            .WithSummary("Get the manifest for UI plugins")
            .WithDescription(
                "Retrieves a map with the installed UI plugin remotes and the associated remoteEntry.js file."
            );
        app.MapGet(
                "/api/plugins/gateway/ui/files/{pluginId}/{version}/{*filePath}",
                ServeUiPluginFile
            )
            .WithTags("Plugins")
            .WithSummary("Serve UI plugin file")
            .WithDescription("Serves static files associated with UI plugins.");
    }

    private static Ok<WebGatewayPluginsResponse> GetGatewayPlugins(
        IManagePluginsUseCase useCase,
        [FromQuery] string? kind
    )
    {
        var installedPlugins = useCase.GetInstalledPlugins();

        if (!string.IsNullOrEmpty(kind))
        {
            installedPlugins = installedPlugins.Where(p =>
                p.Manifest.ExtensionPoints.Any(ep =>
                    string.Equals(ep.Kind, kind, StringComparison.OrdinalIgnoreCase)
                )
            );
        }

        return TypedResults.Ok(
            new WebGatewayPluginsResponse(installedPlugins.Select(WebInstalledGatewayPlugin.Map))
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

    private static Ok<Dictionary<string, string>> GetUiPluginsManifest(
        IManagePluginsUseCase useCase
    )
    {
        var manifest = useCase.GetUiPluginsManifest();
        return TypedResults.Ok(manifest);
    }

    private static IResult ServeUiPluginFile(
        [FromRoute] string pluginId,
        [FromRoute] string version,
        [FromRoute] string filePath,
        IManagePluginsUseCase useCase
    )
    {
        var fullFilePath = useCase.GetUiPluginFilePath(
            new PluginId(pluginId, new Version(version)),
            filePath
        );

        if (fullFilePath == null)
        {
            return TypedResults.NotFound();
        }

        if (
            new FileExtensionContentTypeProvider().TryGetContentType(
                fullFilePath,
                out var contentType
            )
        )
        {
            return TypedResults.PhysicalFile(fullFilePath, contentType);
        }

        return TypedResults.PhysicalFile(fullFilePath, "application/octet-stream");
    }
}

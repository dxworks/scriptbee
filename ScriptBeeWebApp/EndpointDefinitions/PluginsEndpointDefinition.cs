using Microsoft.AspNetCore.Mvc;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class PluginsEndpointDefinition : IEndpointDefinition
{
    // todo allow subclasses to be returned and serialized

    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/plugins", GetLoadedPlugins);
        app.MapGet("/api/plugins/ui", GetLoadedUiPlugins);
        app.MapGet("/api/plugins/available", GetMarketPlugins);
        app.MapPost("/api/plugins/install", InstallPlugin);
        app.MapDelete("/api/plugins/uninstall/{pluginId}/{pluginVersion}", UninstallPlugin);
    }

    public static IEnumerable<PluginManifest> GetLoadedPlugins(IPluginService pluginService,
        [FromQuery] string? type = null)
    {
        if (string.IsNullOrEmpty(type))
        {
            return pluginService.GetPluginManifests();
        }

        return pluginService.GetPluginManifests(type);
    }

    // todo temporary workaround until above todo is fixed
    public static IEnumerable<UiPluginExtensionPoint> GetLoadedUiPlugins(IPluginService pluginService)
    {
        return pluginService.GetExtensionPoints<UiPluginExtensionPoint>();
    }

    public static async Task<IEnumerable<MarketplaceProject>> GetMarketPlugins(IPluginService pluginService,
        CancellationToken cancellationToken = default)
    {
        var baseMarketplacePlugins = await pluginService.GetMarketPlugins(cancellationToken);

        baseMarketplacePlugins = baseMarketplacePlugins
            .Select(plugin =>
            {
                var versions = plugin.Versions.OrderByDescending(version => version.Version);
                return plugin with { Versions = versions.ToList() };
            });

        return baseMarketplacePlugins;
    }

    public static async Task<IResult> InstallPlugin([FromBody] InstallPluginRequest request,
        IPluginService pluginService,
        CancellationToken cancellationToken = default)
    {
        await pluginService.InstallPlugin(request.PluginId, request.Version, cancellationToken);
        return Results.Ok();
    }

    public static IResult UninstallPlugin(string pluginId, string pluginVersion, IPluginService pluginService)
    {
        pluginService.UninstallPlugin(pluginId, pluginVersion);

        return Results.Ok();
    }
}

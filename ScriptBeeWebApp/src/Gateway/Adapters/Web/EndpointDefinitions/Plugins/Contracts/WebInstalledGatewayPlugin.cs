using System.ComponentModel;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

[Description("Represents an installed gateway plugin with its ID and version.")]
public sealed record WebInstalledGatewayPlugin(
    string Id,
    string Version,
    IEnumerable<WebInstalledGatewayPluginExtensionPoint> ExtensionPoints
)
{
    public static WebInstalledGatewayPlugin Map(Plugin plugin)
    {
        return new WebInstalledGatewayPlugin(
            plugin.Id.Name,
            plugin.Id.Version.ToString(),
            plugin.Manifest.ExtensionPoints.Select(WebInstalledGatewayPluginExtensionPoint.Map)
        );
    }
}

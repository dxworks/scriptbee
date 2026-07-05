using System.ComponentModel;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

[Description("Represents the extension points of an installed gateway plugin.")]
public sealed record WebInstalledGatewayPluginExtensionPoint(
    string Kind,
    string? RemoteName,
    string? RemoteEntry,
    IEnumerable<WebInstalledGatewayPluginExtensionPointOutletBase>? Outlets
)
{
    public static WebInstalledGatewayPluginExtensionPoint Map(PluginExtensionPoint extensionPoint)
    {
        return extensionPoint switch
        {
            UiPluginExtensionPoint uiExtensionPoint => new WebInstalledGatewayPluginExtensionPoint(
                uiExtensionPoint.Kind,
                uiExtensionPoint.RemoteName,
                uiExtensionPoint.RemoteEntry,
                uiExtensionPoint.Outlets.Select(
                    WebInstalledGatewayPluginExtensionPointOutletBase.Map
                )
            ),
            _ => new WebInstalledGatewayPluginExtensionPoint(
                extensionPoint.Kind,
                string.Empty,
                string.Empty,
                null
            ),
        };
    }
}

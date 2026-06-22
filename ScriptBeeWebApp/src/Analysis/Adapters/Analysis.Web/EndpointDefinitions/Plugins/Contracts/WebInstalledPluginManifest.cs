using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public record WebInstalledPluginManifest(
    string ApiVersion,
    string Name,
    string? Description,
    string? Author,
    IEnumerable<object> ExtensionPoints
)
{
    public static WebInstalledPluginManifest Map(PluginManifest pluginManifest)
    {
        return new WebInstalledPluginManifest(
            pluginManifest.ApiVersion,
            pluginManifest.Name,
            pluginManifest.Description,
            pluginManifest.Author,
            pluginManifest.ExtensionPoints.Select(MapExtensionPoint)
        );
    }

    private static object MapExtensionPoint(PluginExtensionPoint extensionPoint)
    {
        return extensionPoint switch
        {
            ScriptGeneratorPluginExtensionPoint scriptGeneratorPluginExtensionPoint =>
                new WebScriptGeneratorPluginExtensionPoint
                {
                    EntryPoint = scriptGeneratorPluginExtensionPoint.EntryPoint,
                    Kind = scriptGeneratorPluginExtensionPoint.Kind,
                    Version = scriptGeneratorPluginExtensionPoint.Version,
                    Extension = scriptGeneratorPluginExtensionPoint.Extension,
                    Language = scriptGeneratorPluginExtensionPoint.Language,
                },
            ScriptRunnerPluginExtensionPoint scriptRunnerPluginExtensionPoint =>
                new WebScriptRunnerPluginExtensionPoint
                {
                    Kind = scriptRunnerPluginExtensionPoint.Kind,
                    EntryPoint = scriptRunnerPluginExtensionPoint.EntryPoint,
                    Version = scriptRunnerPluginExtensionPoint.Version,
                    Language = scriptRunnerPluginExtensionPoint.Language,
                    Extension = scriptRunnerPluginExtensionPoint.Extension,
                },
            UiPluginExtensionPoint uiPluginExtensionPoint => new WebUiPluginExtensionPoint
            {
                EntryPoint = uiPluginExtensionPoint.EntryPoint,
                Kind = uiPluginExtensionPoint.Kind,
                Version = uiPluginExtensionPoint.Version,
                RemoteName = uiPluginExtensionPoint.RemoteName,
                RemoteEntry = uiPluginExtensionPoint.RemoteEntry,
                Outlets = uiPluginExtensionPoint.Outlets.Select(o => new WebUiPluginOutlet
                {
                    Type = o.Type,
                    ExposedModule = (o as RoutingOutlet)?.ExposedModule ?? string.Empty,
                    Path = (o as RoutingOutlet)?.Path,
                    Label = (o as RoutingOutlet)?.Label,
                    Nested = (o as RoutingOutlet)?.Nested,
                    ComponentName = (o as RoutingOutlet)?.ComponentName,
                    Icon = (o as SidePanelOutlet)?.Icon,
                    SupportedFileExtensions = (o as FilePreviewerOutlet)?.SupportedFileExtensions,
                }),
            },
            _ => new WebPluginExtensionPoint
            {
                EntryPoint = extensionPoint.EntryPoint,
                Kind = extensionPoint.Kind,
                Version = extensionPoint.Version,
            },
        };
    }
}

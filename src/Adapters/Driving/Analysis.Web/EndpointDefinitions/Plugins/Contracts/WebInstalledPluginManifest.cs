using ScriptBee.Domain.Model.Plugin.Manifest;

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
                },
            UiPluginExtensionPoint uiPluginExtensionPoint => new WebUiPluginExtensionPoint
            {
                EntryPoint = uiPluginExtensionPoint.EntryPoint,
                Kind = uiPluginExtensionPoint.Kind,
                Version = uiPluginExtensionPoint.Version,
                ComponentName = uiPluginExtensionPoint.ComponentName,
                ExposedModule = uiPluginExtensionPoint.ExposedModule,
                Port = uiPluginExtensionPoint.Port,
                RemoteEntry = uiPluginExtensionPoint.RemoteEntry,
                UiPluginType = uiPluginExtensionPoint.UiPluginType,
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

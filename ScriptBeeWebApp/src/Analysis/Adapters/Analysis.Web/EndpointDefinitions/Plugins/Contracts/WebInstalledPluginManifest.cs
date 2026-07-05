using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public record WebInstalledPluginManifest(
    string ApiVersion,
    string Name,
    string? Description,
    string? Author,
    IEnumerable<WebPluginExtensionPoint> ExtensionPoints
)
{
    public static WebInstalledPluginManifest Map(PluginManifest pluginManifest)
    {
        return new WebInstalledPluginManifest(
            pluginManifest.ApiVersion,
            pluginManifest.Name,
            pluginManifest.Description,
            pluginManifest.Author,
            pluginManifest
                .ExtensionPoints.Select(MapExtensionPoint)
                .Where(extensionPoint => extensionPoint is not null)
                .Cast<WebPluginExtensionPoint>()
        );
    }

    private static WebPluginExtensionPoint? MapExtensionPoint(PluginExtensionPoint extensionPoint)
    {
        return extensionPoint switch
        {
            HelperFunctionsPluginExtensionPoint helperFunctionsPluginExtensionPoint =>
                new WebHelperFunctionsPluginExtensionPoint(
                    helperFunctionsPluginExtensionPoint.Kind,
                    helperFunctionsPluginExtensionPoint.EntryPoint,
                    helperFunctionsPluginExtensionPoint.Version
                ),
            LinkerPluginExtensionPoint linkerPluginExtensionPoint =>
                new WebLinkerPluginExtensionPoint(
                    linkerPluginExtensionPoint.Kind,
                    linkerPluginExtensionPoint.EntryPoint,
                    linkerPluginExtensionPoint.Version
                ),
            LoaderPluginExtensionPoint loaderPluginExtensionPoint =>
                new WebLoaderPluginExtensionPoint(
                    loaderPluginExtensionPoint.Kind,
                    loaderPluginExtensionPoint.EntryPoint,
                    loaderPluginExtensionPoint.Version
                ),

            PluginBundleExtensionPoint pluginBundleExtensionPoint =>
                new WebNestedPluginExtensionPoint(
                    pluginBundleExtensionPoint.Kind,
                    pluginBundleExtensionPoint.EntryPoint,
                    pluginBundleExtensionPoint.Version
                ),
            ScriptGeneratorPluginExtensionPoint scriptGeneratorPluginExtensionPoint =>
                new WebScriptGeneratorPluginExtensionPoint(
                    scriptGeneratorPluginExtensionPoint.Kind,
                    scriptGeneratorPluginExtensionPoint.EntryPoint,
                    scriptGeneratorPluginExtensionPoint.Version,
                    scriptGeneratorPluginExtensionPoint.Language,
                    scriptGeneratorPluginExtensionPoint.Extension
                ),
            ScriptRunnerPluginExtensionPoint scriptRunnerPluginExtensionPoint =>
                new WebScriptRunnerPluginExtensionPoint(
                    scriptRunnerPluginExtensionPoint.Kind,
                    scriptRunnerPluginExtensionPoint.EntryPoint,
                    scriptRunnerPluginExtensionPoint.Version,
                    scriptRunnerPluginExtensionPoint.Language,
                    scriptRunnerPluginExtensionPoint.Extension
                ),
            UiPluginExtensionPoint uiPluginExtensionPoint => new WebUiPluginExtensionPoint(
                uiPluginExtensionPoint.Kind,
                uiPluginExtensionPoint.EntryPoint,
                uiPluginExtensionPoint.Version,
                uiPluginExtensionPoint.RemoteName,
                uiPluginExtensionPoint.RemoteEntry,
                uiPluginExtensionPoint.Outlets.Select(
                    WebInstalledPluginExtensionPointOutletBase.Map
                )
            ),
            _ => null,
        };
    }
}

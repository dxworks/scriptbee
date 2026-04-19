using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Plugins.Installer;

internal static class BundleExtensionPointUtils
{
    internal static IEnumerable<PluginId> GetPluginExtensionPointsIds(
        IPluginReader pluginReader,
        string bundleFolder
    )
    {
        var installedBundle = pluginReader.ReadPlugin(bundleFolder);

        return installedBundle is null
            ? []
            : installedBundle
                .Manifest.ExtensionPoints.Where(point => point.Kind == PluginKind.Plugin)
                .Select(extensionPoint => new PluginId(
                    extensionPoint.EntryPoint,
                    new Version(extensionPoint.Version)
                ));
    }
}

using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Installer;

public class BundlePluginUninstaller(
    IPluginReader pluginReader,
    IPluginUninstaller pluginUninstaller,
    ILogger<BundlePluginUninstaller> logger
) : IBundlePluginUninstaller
{
    public List<PluginId> Uninstall(PluginId pluginId, string pluginFolderPath)
    {
        logger.LogInformation(
            "Uninstalling plugin {PluginName} version {Version} from {PluginFolderPath}",
            pluginId.Name,
            pluginId.Version,
            pluginFolderPath
        );

        var bundleFolder = Path.Combine(pluginFolderPath, pluginId.GetFullyQualifiedName());

        pluginUninstaller.Uninstall(bundleFolder);
        var uninstalledVersions = new List<PluginId> { pluginId };

        var plugin = pluginReader.ReadPlugin(bundleFolder);
        if (plugin is null)
        {
            return uninstalledVersions;
        }

        foreach (
            var id in BundleExtensionPointUtils.GetPluginExtensionPointsIds(
                pluginReader,
                bundleFolder
            )
        )
        {
            var versions = Uninstall(id, pluginFolderPath);
            uninstalledVersions.AddRange(versions);
        }

        return uninstalledVersions;
    }
}

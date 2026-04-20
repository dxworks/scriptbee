using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Installer;

public class BundlePluginUninstaller(
    IPluginReader pluginReader,
    IPluginUninstaller pluginUninstaller,
    IPluginPathProvider pluginPathProvider,
    ILogger<BundlePluginUninstaller> logger
) : IBundlePluginUninstaller
{
    public List<PluginId> Uninstall(PluginId pluginId)
    {
        logger.LogInformation(
            "Uninstalling plugin {PluginName} version {Version}",
            pluginId.Name,
            pluginId.Version
        );

        var bundleFolder = GetPluginPath(pluginId);

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
            var versions = Uninstall(id);
            uninstalledVersions.AddRange(versions);
        }

        return uninstalledVersions;
    }

    private string GetPluginPath(PluginId pluginId)
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        return Path.Combine(pluginFolderPath, pluginId.GetFullyQualifiedName());
    }
}

using Microsoft.Extensions.Logging;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Plugins.Installer;

public class BundlePluginUninstaller(
    IFileService fileService,
    IPluginReader pluginReader,
    IPluginUninstaller pluginUninstaller,
    IPluginPathProvider pluginPathProvider,
    ILogger<BundlePluginUninstaller> logger
) : IBundlePluginUninstaller
{
    public List<(string PluginId, string Version)> Uninstall(string pluginId, string version)
    {
        logger.LogInformation(
            "Uninstalling plugin {PluginId} version {Version}",
            pluginId,
            version
        );

        return UninstallBundle(pluginId, version);
    }

    private List<(string PluginId, string Version)> UninstallBundle(string bundleId, string version)
    {
        var bundleFolder = GetPluginPath(bundleId, version);

        pluginUninstaller.Uninstall(bundleFolder);
        var uninstalledVersions = new List<(string PluginId, string Version)>
        {
            (bundleId, version),
        };

        var plugin = pluginReader.ReadPlugin(bundleFolder);
        if (plugin is null)
        {
            return uninstalledVersions;
        }

        foreach (var extensionPoint in GetPluginExtensionPoints(bundleFolder))
        {
            var versions = UninstallBundle(extensionPoint.EntryPoint, extensionPoint.Version);
            uninstalledVersions.AddRange(versions);
        }

        return uninstalledVersions;
    }

    private string GetPluginPath(string pluginId, string version)
    {
        var pluginName = PluginNameGenerator.GetPluginName(pluginId, version);
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        return fileService.CombinePaths(pluginFolderPath, pluginName);
    }

    private IEnumerable<PluginExtensionPoint> GetPluginExtensionPoints(string bundleFolder)
    {
        var installedBundle = pluginReader.ReadPlugin(bundleFolder);

        return installedBundle is null
            ? []
            : installedBundle.Manifest.ExtensionPoints.Where(point =>
                point.Kind == PluginKind.Plugin
            );
    }
}

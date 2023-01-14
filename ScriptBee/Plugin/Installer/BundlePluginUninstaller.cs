using System.Collections.Generic;
using System.Linq;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Manifest;
using Serilog;

namespace ScriptBee.Plugin.Installer;

public class BundlePluginUninstaller : IBundlePluginUninstaller
{
    private readonly IFileService _fileService;
    private readonly IPluginReader _pluginReader;
    private readonly IPluginUninstaller _pluginUninstaller;
    private readonly ILogger _logger;

    public BundlePluginUninstaller(IFileService fileService, IPluginReader pluginReader,
        IPluginUninstaller pluginUninstaller, ILogger logger)
    {
        _fileService = fileService;
        _pluginReader = pluginReader;
        _pluginUninstaller = pluginUninstaller;
        _logger = logger;
    }

    public List<(string PluginId, string Version)> Uninstall(string pluginId, string version)
    {
        _logger.Information("Uninstalling plugin {PluginId} version {Version}", pluginId, version);

        return UninstallBundle(pluginId, version);
    }

    private List<(string PluginId, string Version)> UninstallBundle(string bundleId, string version)
    {
        var bundleFolder = GetPluginPath(bundleId, version);

        _pluginUninstaller.Uninstall(bundleFolder);
        var uninstalledVersions = new List<(string PluginId, string Version)>
        {
            (bundleId, version)
        };

        var plugin = _pluginReader.ReadPlugin(bundleFolder);
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
        return _fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);
    }

    private IEnumerable<PluginExtensionPoint> GetPluginExtensionPoints(string bundleFolder)
    {
        var installedBundle = _pluginReader.ReadPlugin(bundleFolder);

        return installedBundle is null
            ? Enumerable.Empty<PluginExtensionPoint>()
            : installedBundle.Manifest.ExtensionPoints.Where(point => point.Kind == PluginKind.Plugin);
    }
}

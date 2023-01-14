using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Config;
using ScriptBee.Plugin.Manifest;
using Serilog;

namespace ScriptBee.Plugin.Installer;

public class BundlePluginInstaller : IBundlePluginInstaller
{
    private readonly IPluginReader _pluginReader;
    private readonly ISimplePluginInstaller _simplePluginInstaller;
    private readonly IPluginUninstaller _pluginUninstaller;
    private readonly IPluginUrlFetcher _pluginUrlFetcher;
    private readonly ILogger _logger;

    public BundlePluginInstaller(IPluginReader pluginReader, ISimplePluginInstaller simplePluginInstaller,
        IPluginUninstaller pluginUninstaller, IPluginUrlFetcher pluginUrlFetcher, ILogger logger)
    {
        _pluginReader = pluginReader;
        _simplePluginInstaller = simplePluginInstaller;
        _pluginUninstaller = pluginUninstaller;
        _logger = logger;
        _pluginUrlFetcher = pluginUrlFetcher;
    }

    public async Task<List<string>> Install(string pluginId, string version,
        CancellationToken cancellationToken = default)
    {
        var (installedPluginFolders, pluginFoldersToUninstall) =
            await InstallPluginBundle(pluginId, version, cancellationToken);

        foreach (var pathToPlugin in pluginFoldersToUninstall)
        {
            _pluginUninstaller.Uninstall(pathToPlugin);
        }

        return installedPluginFolders;
    }

    private async Task<(List<string> installedPluginFolders, List<string> pluginFoldersToUninstall)>
        InstallPluginBundle(string pluginId,
            string version,
            CancellationToken cancellationToken = default)
    {
        var (bundleFolder, existingVersionsToUninstall) = await InstallPlugin(pluginId, version, cancellationToken);

        if (string.IsNullOrEmpty(bundleFolder))
        {
            return (new List<string>(), new List<string>());
        }

        var installedPluginsPaths = new List<string>
        {
            bundleFolder
        };
        var pluginFoldersToUninstall = existingVersionsToUninstall.Select(p => p.FolderPath).ToList();

        var tasks = GetPluginExtensionPoints(bundleFolder)
            .Select(extensionPoint =>
                InstallPluginBundle(extensionPoint.EntryPoint, extensionPoint.Version, cancellationToken))
            .ToList();

        try
        {
            var installedBundlesTuples = await Task.WhenAll(tasks);

            foreach (var (installedPluginFolders, pluginFoldersToUninstallForExtensionPoint) in installedBundlesTuples)
            {
                installedPluginsPaths.AddRange(installedPluginFolders);
                pluginFoldersToUninstall.AddRange(pluginFoldersToUninstallForExtensionPoint);
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while installing plugin {PluginId} version {Version}", pluginId, version);

            foreach (var installedPluginPath in installedPluginsPaths)
            {
                _pluginUninstaller.ForceUninstall(installedPluginPath);
            }

            throw;
        }

        return (installedPluginsPaths, pluginFoldersToUninstall);
    }

    private async Task<(string? pluginFolder, List<Models.Plugin> installedPluginVersions)> InstallPlugin(
        string pluginId, string version, CancellationToken cancellationToken)
    {
        var installedPluginVersions = _pluginReader.ReadPlugins(ConfigFolders.PathToPlugins)
            .Where(p => p.Id == pluginId)
            .ToList();

        if (!IsLatestVersion(installedPluginVersions, version))
        {
            _logger.Information("A newer version of plugin {PluginId} is already installed", pluginId);
            return (null, installedPluginVersions);
        }

        var url = await _pluginUrlFetcher.GetPluginUrlAsync(pluginId, version, cancellationToken);
        var pluginFolder = await _simplePluginInstaller.Install(url, pluginId, version, cancellationToken);

        return (pluginFolder, installedPluginVersions);
    }

    private IEnumerable<PluginExtensionPoint> GetPluginExtensionPoints(string bundleFolder)
    {
        var installedBundle = _pluginReader.ReadPlugin(bundleFolder);

        return installedBundle is null
            ? Enumerable.Empty<PluginExtensionPoint>()
            : installedBundle.Manifest.ExtensionPoints.Where(point => point.Kind == PluginKind.Plugin);
    }

    private static bool IsLatestVersion(IEnumerable<Models.Plugin> plugins, string versionString)
    {
        var version = new Version(versionString);
        return plugins.All(plugin => plugin.Version < version);
    }
}

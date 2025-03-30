using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Plugins.Installer;

namespace ScriptBee.Persistence.File.Plugin.Installer;

public class BundlePluginInstaller(
    IPluginReader pluginReader,
    ISimplePluginInstaller simplePluginInstaller,
    IPluginUninstaller pluginUninstaller,
    IPluginUrlFetcher pluginUrlFetcher,
    ILogger<BundlePluginInstaller> logger
) : IBundlePluginInstaller
{
    public async Task<List<string>> Install(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    )
    {
        var (installedPluginFolders, pluginFoldersToUninstall) = await InstallPluginBundle(
            pluginId,
            version,
            cancellationToken
        );

        foreach (var pathToPlugin in pluginFoldersToUninstall)
        {
            pluginUninstaller.Uninstall(pathToPlugin);
        }

        return installedPluginFolders;
    }

    private async Task<(
        List<string> installedPluginFolders,
        List<string> pluginFoldersToUninstall
    )> InstallPluginBundle(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    )
    {
        var (bundleFolder, existingVersionsToUninstall) = await InstallPlugin(
            pluginId,
            version,
            cancellationToken
        );

        if (string.IsNullOrEmpty(bundleFolder))
        {
            return ([], []);
        }

        var installedPluginsPaths = new List<string> { bundleFolder };
        var pluginFoldersToUninstall = existingVersionsToUninstall
            .Select(p => p.FolderPath)
            .ToList();

        var tasks = GetPluginExtensionPoints(bundleFolder)
            .Select(extensionPoint =>
                InstallPluginBundle(
                    extensionPoint.EntryPoint,
                    extensionPoint.Version,
                    cancellationToken
                )
            )
            .ToList();

        try
        {
            var installedBundlesTuples = await Task.WhenAll(tasks);

            foreach (
                var (
                    installedPluginFolders,
                    pluginFoldersToUninstallForExtensionPoint
                ) in installedBundlesTuples
            )
            {
                installedPluginsPaths.AddRange(installedPluginFolders);
                pluginFoldersToUninstall.AddRange(pluginFoldersToUninstallForExtensionPoint);
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Error while installing plugin {PluginId} version {Version}",
                pluginId,
                version
            );

            foreach (var installedPluginPath in installedPluginsPaths)
            {
                pluginUninstaller.ForceUninstall(installedPluginPath);
            }

            throw;
        }

        return (installedPluginsPaths, pluginFoldersToUninstall);
    }

    private async Task<(
        string? pluginFolder,
        List<Domain.Model.Plugin.Plugin> installedPluginVersions
    )> InstallPlugin(string pluginId, string version, CancellationToken cancellationToken)
    {
        var installedPluginVersions = pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Where(p => p.Id == pluginId)
            .ToList();

        if (!IsLatestVersion(installedPluginVersions, version))
        {
            logger.LogInformation(
                "A newer version of plugin {PluginId} is already installed",
                pluginId
            );
            return (null, installedPluginVersions);
        }

        var url = pluginUrlFetcher.GetPluginUrl(pluginId, version);
        var pluginFolder = await simplePluginInstaller.Install(
            url,
            pluginId,
            version,
            cancellationToken
        );

        return (pluginFolder, installedPluginVersions);
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

    private static bool IsLatestVersion(
        IEnumerable<Domain.Model.Plugin.Plugin> plugins,
        string versionString
    )
    {
        var version = new Version(versionString);
        return plugins.All(plugin => plugin.Version < version);
    }
}

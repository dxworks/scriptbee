using System.Text;
using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Marketplace.Client;

namespace ScriptBee.Common.Plugins.Installer;

using BundleInstallResult = OneOf<
    InstallBundleResult,
    PluginVersionExistsError,
    PluginInstallationError
>;
using PluginInstallResult = OneOf<
    InstallPluginResult,
    PluginVersionExistsError,
    PluginInstallationError
>;

public class BundlePluginInstaller(
    IPluginReader pluginReader,
    ISimplePluginInstaller simplePluginInstaller,
    IPluginUninstaller pluginUninstaller,
    IPluginUrlFetcher pluginUrlFetcher,
    IPluginPathProvider pluginPathProvider,
    ILogger<BundlePluginInstaller> logger
) : IBundlePluginInstaller
{
    public async Task<
        OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
    > Install(string pluginId, string version, CancellationToken cancellationToken = default)
    {
        var result = await InstallPluginBundle(pluginId, version, cancellationToken);

        return result.Match<OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>>(
            installResult =>
            {
                var (installedPluginFolders, pluginFoldersToUninstall) = installResult;

                foreach (var pathToPlugin in pluginFoldersToUninstall)
                {
                    pluginUninstaller.Uninstall(pathToPlugin);
                }

                return installedPluginFolders;
            },
            error => error,
            error => error
        );
    }

    private async Task<BundleInstallResult> InstallPluginBundle(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    )
    {
        var result = await InstallPlugin(pluginId, version, cancellationToken);

        return await result.Match<Task<BundleInstallResult>>(
            async pluginResult =>
                await InstallPluginBundle(pluginId, version, pluginResult, cancellationToken),
            error =>
                Task.FromResult<BundleInstallResult>(
                    new PluginInstallationError(error.Name, error.Version)
                ),
            error => Task.FromResult<BundleInstallResult>(error)
        );
    }

    private async Task<BundleInstallResult> InstallPluginBundle(
        string pluginId,
        string version,
        InstallPluginResult installPluginResult,
        CancellationToken cancellationToken = default
    )
    {
        var (bundleFolder, existingVersionsToUninstall) = installPluginResult;

        if (string.IsNullOrEmpty(bundleFolder))
        {
            return new InstallBundleResult([], []);
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

        var errorMessageBuilder = new StringBuilder();
        var installedBundlesTuples = await Task.WhenAll(tasks);

        foreach (var installResult in installedBundlesTuples)
        {
            installResult.Switch(
                result =>
                {
                    installedPluginsPaths.AddRange(result.InstalledPluginFolders);
                    pluginFoldersToUninstall.AddRange(result.PluginFoldersToUninstall);
                },
                error =>
                    errorMessageBuilder.AppendLine(
                        $"Plugin {error.Name} version {error.Version} could not be installed."
                    ),
                error =>
                    errorMessageBuilder.AppendLine(
                        $"Plugin {error.Name} version {error.Version} could not be installed."
                    )
            );
        }

        if (errorMessageBuilder.Length <= 0)
        {
            return new InstallBundleResult(installedPluginsPaths, pluginFoldersToUninstall);
        }

        logger.LogError("{Message}", errorMessageBuilder.ToString());

        foreach (var installedPluginPath in installedPluginsPaths)
        {
            pluginUninstaller.ForceUninstall(installedPluginPath);
        }

        return new PluginInstallationError(pluginId, version);
    }

    private async Task<PluginInstallResult> InstallPlugin(
        string pluginId,
        string version,
        CancellationToken cancellationToken
    )
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        var installedPluginVersions = pluginReader
            .ReadPlugins(pluginFolderPath)
            .Where(p => p.Id == pluginId)
            .ToList();

        if (!IsLatestVersion(installedPluginVersions, version))
        {
            logger.LogInformation(
                "A newer version of plugin {PluginId} is already installed",
                pluginId
            );
            return new InstallPluginResult(null, installedPluginVersions);
        }

        var result = await pluginUrlFetcher.GetPluginUrl(pluginId, version, cancellationToken);

        return await result.Match<Task<PluginInstallResult>>(
            async url =>
                await InstallPlugin(
                    url,
                    pluginId,
                    version,
                    installedPluginVersions,
                    cancellationToken
                ),
            error =>
                Task.FromResult<PluginInstallResult>(
                    new PluginInstallationError(error.PluginId, version)
                ),
            error =>
                Task.FromResult<PluginInstallResult>(
                    new PluginInstallationError(error.PluginId, error.Version)
                )
        );
    }

    private async Task<PluginInstallResult> InstallPlugin(
        string url,
        string pluginId,
        string version,
        List<Plugin> installedPluginVersions,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await simplePluginInstaller.Install(
                url,
                pluginId,
                version,
                cancellationToken
            );

            return result.Match<PluginInstallResult>(
                pluginFolder => new InstallPluginResult(pluginFolder, installedPluginVersions),
                error => error,
                error => error
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while installing plugin {PluginId} version {Version}",
                pluginId,
                version
            );
            return new PluginInstallationError(pluginId, version);
        }
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

    private static bool IsLatestVersion(IEnumerable<Plugin> plugins, string versionString)
    {
        var version = new Version(versionString);
        return plugins.All(plugin => plugin.Version < version);
    }
}

internal sealed record InstallPluginResult(
    string? pluginFolder,
    List<Plugin> installedPluginVersions
);

internal sealed record InstallBundleResult(
    List<string> InstalledPluginFolders,
    List<string> PluginFoldersToUninstall
);

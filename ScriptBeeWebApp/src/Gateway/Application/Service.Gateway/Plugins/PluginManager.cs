using Microsoft.Extensions.Logging;
using ScriptBee.Common.Extensions;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Installer;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Plugins;

public sealed class PluginManager(
    IPluginReader pluginReader,
    IPluginLoader pluginLoader,
    IBundlePluginInstaller bundlePluginInstaller,
    IGatewayPluginPathProvider pluginPathProvider,
    ILogger<PluginManager> logger
) : IManagePluginsUseCase
{
    public void LoadPlugins()
    {
        var pluginFolderPath = pluginPathProvider.GetInstallationFolderPath();
        logger.LogInformation("Loading plugins from {Folder}", pluginFolderPath);

        var plugins = pluginReader.ReadPlugins(pluginFolderPath);

        foreach (var plugin in plugins)
        {
            try
            {
                pluginLoader.Load(plugin);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to load plugin {Plugin}", plugin);
            }
        }
    }

    public async Task Install(PluginId pluginId, CancellationToken cancellationToken)
    {
        var result = await bundlePluginInstaller.Install(pluginId, cancellationToken);

        await result.Match<Task>(
            plugins =>
            {
                foreach (var id in plugins)
                {
                    var cachePath = Path.Combine(
                        pluginPathProvider.GetPathToPlugins(),
                        id.GetFullyQualifiedName()
                    );
                    var activePath = Path.Combine(
                        pluginPathProvider.GetInstallationFolderPath(),
                        id.GetFullyQualifiedName()
                    );

                    if (!Directory.Exists(cachePath))
                    {
                        continue;
                    }

                    new DirectoryInfo(activePath).DeleteIfExists();
                    new DirectoryInfo(cachePath).CopyTo(activePath);

                    var plugin = pluginReader.ReadPlugin(activePath);
                    if (plugin != null)
                    {
                        pluginLoader.Load(plugin);
                    }
                }

                return Task.CompletedTask;
            },
            error =>
            {
                logger.LogError("Failed to install plugin {PluginId}: {Error}", pluginId, error);
                return Task.CompletedTask;
            }
        );
    }

    public void Uninstall(PluginId pluginId)
    {
        var activePath = Path.Combine(
            pluginPathProvider.GetInstallationFolderPath(),
            pluginId.GetFullyQualifiedName()
        );

        pluginLoader.Unload(pluginId);

        new DirectoryInfo(activePath).DeleteIfExists();
    }

    public IEnumerable<Plugin> GetInstalledPlugins()
    {
        var activePath = pluginPathProvider.GetInstallationFolderPath();

        return pluginReader.ReadPlugins(activePath);
    }

    public Dictionary<string, string> GetUiPluginsManifest()
    {
        var manifest = new Dictionary<string, string>();

        var uiPlugins = GetInstalledPlugins()
            .Where(p => p.Manifest.ExtensionPoints.Any(ep => ep is UiPluginExtensionPoint));

        foreach (var plugin in uiPlugins)
        {
            foreach (var ep in plugin.Manifest.ExtensionPoints)
            {
                if (ep is not UiPluginExtensionPoint uiEp)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(uiEp.EntryPoint))
                {
                    manifest[uiEp.RemoteName] =
                        $"/api/plugins/gateway/ui/files/{plugin.Id.Name}/{plugin.Id.Version}/{uiEp.RemoteEntry}";
                }
                else
                {
                    manifest[uiEp.RemoteName] = uiEp.RemoteEntry;
                }
            }
        }

        return manifest;
    }

    public string? GetUiPluginFilePath(PluginId pluginId, string filePath)
    {
        var activePath = pluginPathProvider.GetInstallationFolderPath();
        var pluginFolder = Path.Combine(activePath, pluginId.GetFullyQualifiedName());

        var installedPlugin = GetInstalledPlugins().FirstOrDefault(p => p.Id == pluginId);

        var uiEp = installedPlugin
            ?.Manifest.ExtensionPoints.OfType<UiPluginExtensionPoint>()
            .FirstOrDefault();

        if (uiEp == null || string.IsNullOrWhiteSpace(uiEp.EntryPoint))
        {
            return null;
        }

        var entryPointFolder = Path.Combine(pluginFolder, uiEp.EntryPoint);
        var fullFilePath = Path.GetFullPath(Path.Combine(entryPointFolder, filePath));

        if (!fullFilePath.StartsWith(Path.GetFullPath(entryPointFolder)))
        {
            return null;
        }

        return !File.Exists(fullFilePath) ? null : fullFilePath;
    }
}

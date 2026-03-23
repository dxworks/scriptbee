using Microsoft.Extensions.Logging;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Plugins.Installer;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin;

public class InstallPluginService(
    IBundlePluginInstaller bundlePluginInstaller,
    IPluginLoader pluginLoader,
    IPluginReader pluginReader,
    ILogger<UninstallPluginService> logger
) : IInstallPluginUseCase
{
    public async Task InstallPlugin(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    )
    {
        var installPluginPaths = await bundlePluginInstaller.Install(
            pluginId,
            version,
            cancellationToken
        );

        foreach (var installPluginPath in installPluginPaths)
        {
            var plugin = pluginReader.ReadPlugin(installPluginPath);
            if (plugin is null)
            {
                logger.LogWarning(
                    "Plugin Manifest from {Path} could not be read",
                    installPluginPath
                );
                continue;
            }

            pluginLoader.Load(plugin);
        }
    }
}

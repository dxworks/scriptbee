using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Installer;
using ScriptBee.UseCases.Plugin;
using ScriptBee.UseCases.Plugin.Errors;
using PluginInstallationError = ScriptBee.UseCases.Plugin.Errors.PluginInstallationError;

namespace ScriptBee.Service.Plugin;

public class InstallPluginService(
    IBundlePluginInstaller bundlePluginInstaller,
    IPluginLoader pluginLoader,
    IPluginReader pluginReader,
    ILogger<InstallPluginService> logger
) : IInstallPluginUseCase
{
    public async Task<OneOf<Success, InvalidPluginError, PluginInstallationError>> InstallPlugin(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    )
    {
        var result = await bundlePluginInstaller.Install(pluginId, version, cancellationToken);

        return result.Match<OneOf<Success, InvalidPluginError, PluginInstallationError>>(
            installPluginPaths =>
            {
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

                return new Success();
            },
            error => new InvalidPluginError(error.Name, error.Version),
            error => new PluginInstallationError(error.Name, error.Version)
        );
    }
}

using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Analysis;
using ScriptBee.UseCases.Analysis.Errors;

namespace ScriptBee.Service.Analysis;

public class InstallPluginService(
    IProjectManager projectManager,
    IPluginReader pluginReader,
    IPluginLoader pluginLoader,
    IPluginPathProvider pluginPathProvider,
    ILogger<InstallPluginService> logger
) : IInstallPluginUseCase
{
    public OneOf<Success, InvalidPluginError, PluginInstallationError> InstallPlugin(
        PluginId pluginId
    )
    {
        logger.LogInformation(
            "Installing plugin {PluginName} {PluginVersion} on analysis instance",
            pluginId.Name,
            pluginId.Version
        );

        try
        {
            var projectId = ProjectId.FromValue(projectManager.GetProject().Id);

            var plugin =
                pluginReader.ReadPlugin(pluginPathProvider.GetPathToPlugins(projectId), pluginId)
                ?? pluginReader.ReadPlugin(pluginPathProvider.GetPathToPlugins(), pluginId);

            if (plugin is null)
            {
                logger.LogWarning(
                    "Plugin {PluginName} {PluginVersion} not found in project or global plugin paths",
                    pluginId.Name,
                    pluginId.Version
                );
                return new InvalidPluginError(pluginId);
            }

            pluginLoader.Load(plugin);

            logger.LogInformation(
                "Plugin {PluginName} {PluginVersion} installed on analysis instance",
                pluginId.Name,
                pluginId.Version
            );

            return new Success();
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to install plugin {PluginName} {PluginVersion} on analysis instance",
                pluginId.Name,
                pluginId.Version
            );
            return new PluginInstallationError(pluginId);
        }
    }
}

using Microsoft.Extensions.Logging;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Plugins;

public sealed class PluginManager(
    IPluginReader pluginReader,
    IPluginLoader pluginLoader,
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
}

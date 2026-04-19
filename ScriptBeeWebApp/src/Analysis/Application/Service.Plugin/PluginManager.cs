using Microsoft.Extensions.Logging;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin;

public class PluginManager(
    IPluginReader pluginReader,
    IPluginLoader pluginLoader,
    IPluginPathProvider pluginPathProvider,
    ILogger<PluginManager> logger
) : IManagePluginsUseCase
{
    public void LoadPlugins()
    {
        // todo filter custom plugin definitions first
        // todo iterate over all plugin definitions and load them
        logger.LogInformation("Loading plugins");

        var plugins = pluginReader.ReadPlugins(pluginPathProvider.GetPathToPlugins());

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

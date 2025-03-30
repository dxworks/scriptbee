using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Config;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin;

public class PluginManager(
    IPluginReader pluginReader,
    IPluginLoader pluginLoader,
    ILogger<PluginManager> logger
)
{
    public void LoadPlugins()
    {
        // todo filter custom plugin definitions first
        // todo iterate over all plugin definitions and load them
        logger.LogInformation("Loading plugins");

        var plugins = pluginReader.ReadPlugins(ConfigFolders.PathToPlugins);

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

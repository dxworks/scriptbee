using ScriptBee.FileManagement;
using Serilog;

namespace ScriptBee.Plugin;

// todo add tests
public class PluginManager
{
    private readonly ILogger _logger;
    private readonly IPluginReader _pluginReader;
    private readonly IPluginLoaderFactory _pluginLoaderFactory;

    public PluginManager(ILogger logger, IPluginReader pluginReader, IPluginLoaderFactory pluginLoaderFactory)
    {
        _logger = logger;
        _pluginReader = pluginReader;
        _pluginLoaderFactory = pluginLoaderFactory;
    }

    // todo move to task
    public void LoadPlugins(string allPluginsFolder)
    {
        // todo filter custom plugin definitions first
        // todo iterate over all plugin definitions and load them
        var plugins = _pluginReader.ReadPlugins(allPluginsFolder);

        foreach (var plugin in plugins)
        {
            var pluginLoader = _pluginLoaderFactory.GetPluginLoader(plugin);
            if (pluginLoader is null)
            {
                _logger.Warning("Unknown plugin type {plugin}", plugin);
            }
            else
            {
                pluginLoader.Load(plugin);
            }
        }
    }
}

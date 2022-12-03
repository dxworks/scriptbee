using System;
using Serilog;

namespace ScriptBee.Plugin;

// todo add tests
public class PluginManager
{
    private readonly ILogger _logger;
    private readonly IPluginReader _pluginReader;
    private readonly IPluginLoader _pluginLoader;

    public PluginManager(ILogger logger, IPluginReader pluginReader, IPluginLoader pluginLoader)
    {
        _logger = logger;
        _pluginReader = pluginReader;
        _pluginLoader = pluginLoader;
    }

    // todo move to task
    public void LoadPlugins(string allPluginsFolder)
    {
        _pluginReader.ClearDeletePluginsFolder(allPluginsFolder);

        // todo filter custom plugin definitions first
        // todo iterate over all plugin definitions and load them
        var plugins = _pluginReader.ReadPlugins(allPluginsFolder);

        foreach (var plugin in plugins)
        {
            try
            {
                _pluginLoader.Load(plugin);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to load plugin {plugin}", plugin);
            }
        }
    }
}

using System;
using ScriptBee.Config;
using ScriptBee.Plugin.Installer;
using Serilog;

namespace ScriptBee.Plugin;

// todo add tests
public class PluginManager
{
    private readonly IPluginReader _pluginReader;
    private readonly IPluginLoader _pluginLoader;
    private readonly IPluginUninstaller _pluginUninstaller;
    private readonly ILogger _logger;

    public PluginManager(IPluginReader pluginReader, IPluginLoader pluginLoader, IPluginUninstaller pluginUninstaller, ILogger logger)
    {
        _pluginReader = pluginReader;
        _pluginLoader = pluginLoader;
        _pluginUninstaller = pluginUninstaller;
        _logger = logger;
    }

    // todo move to task
    public void LoadPlugins()
    {
        _pluginUninstaller.DeleteMarkedPlugins();

        // todo filter custom plugin definitions first
        // todo iterate over all plugin definitions and load them
        var plugins = _pluginReader.ReadPlugins(ConfigFolders.PathToPlugins);

        foreach (var plugin in plugins)
        {
            try
            {
                _pluginLoader.Load(plugin);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to load plugin {Plugin}", plugin);
            }
        }
    }
}

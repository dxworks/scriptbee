using System;
using ScriptBee.Config;
using ScriptBee.Plugin.Installer;
using ScriptBee.ProjectContext;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginManager
{
    private readonly IPluginReader _pluginReader;
    private readonly IPluginLoader _pluginLoader;
    private readonly IPluginUninstaller _pluginUninstaller;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly ILogger _logger;

    public PluginManager(IPluginReader pluginReader, IPluginLoader pluginLoader, IPluginUninstaller pluginUninstaller,
        IProjectFileStructureManager projectFileStructureManager, ILogger logger)
    {
        _pluginReader = pluginReader;
        _pluginLoader = pluginLoader;
        _pluginUninstaller = pluginUninstaller;
        _logger = logger;
        _projectFileStructureManager = projectFileStructureManager;
    }

    // todo move to task
    public void LoadPlugins()
    {
        _logger.Information("Create ScriptBee folder structure");
        _projectFileStructureManager.CreateScriptBeeFolderStructure();

        _logger.Information("Delete marked plugins");
        _pluginUninstaller.DeleteMarkedPlugins();

        // todo filter custom plugin definitions first
        // todo iterate over all plugin definitions and load them
        _logger.Information("Loading plugins");

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

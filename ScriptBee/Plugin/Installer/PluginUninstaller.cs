using System;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using Serilog;

namespace ScriptBee.Plugin.Installer;

public class PluginUninstaller : IPluginUninstaller
{
    private const string DeletePluginFolderFileName = "__delete.txt";

    private readonly IFileService _fileService;
    private readonly ILogger _logger;

    public PluginUninstaller(IFileService fileService, ILogger logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    public void ForceUninstall(string pathToPlugin)
    {
        _logger.Information("Force uninstalling plugin from {PathToPlugin}", pathToPlugin);
        _fileService.DeleteDirectory(pathToPlugin);
    }

    public void Uninstall(string pathToPlugin)
    {
        _logger.Information("Marking plugin for deletion: {PathToPlugin}", pathToPlugin);

        var deletePluginFolderFilePath = GetDeletePluginFolderFilePath();

        _fileService.AppendTextToFile(deletePluginFolderFilePath, pathToPlugin);
    }

    public void DeleteMarkedPlugins()
    {
        var deletePluginFolderFilePath = GetDeletePluginFolderFilePath();

        if (!_fileService.FileExists(deletePluginFolderFilePath))
        {
            return;
        }

        var pluginsToDelete = _fileService.ReadAllLines(deletePluginFolderFilePath);
        var shouldRemoveMarkedPluginsFile = true;

        foreach (var pluginToDelete in pluginsToDelete)
        {
            _logger.Information("Deleting plugin: {PluginToDelete}", pluginToDelete);

            try
            {
                _fileService.DeleteDirectory(pluginToDelete);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error deleting plugin '{PluginToDelete};", pluginToDelete);
                shouldRemoveMarkedPluginsFile = false;
            }
        }

        if (shouldRemoveMarkedPluginsFile)
        {
            _fileService.DeleteFile(deletePluginFolderFilePath);
        }
    }

    private string GetDeletePluginFolderFilePath()
    {
        return _fileService.CombinePaths(ConfigFolders.PathToPlugins, DeletePluginFolderFileName);
    }
}

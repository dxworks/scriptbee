using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Config;

namespace ScriptBee.Persistence.File.Plugin.Installer;

public class PluginUninstaller(IFileService fileService, ILogger<PluginUninstaller> logger)
    : IPluginUninstaller
{
    private const string DeletePluginFolderFileName = "__delete.txt";

    public void ForceUninstall(string pathToPlugin)
    {
        logger.LogInformation("Force uninstalling plugin from {PathToPlugin}", pathToPlugin);
        fileService.DeleteDirectory(pathToPlugin);
    }

    public void Uninstall(string pathToPlugin)
    {
        logger.LogInformation("Marking plugin for deletion: {PathToPlugin}", pathToPlugin);

        var deletePluginFolderFilePath = GetDeletePluginFolderFilePath();

        fileService.AppendTextToFile(deletePluginFolderFilePath, pathToPlugin);
    }

    public void DeleteMarkedPlugins()
    {
        var deletePluginFolderFilePath = GetDeletePluginFolderFilePath();

        if (!fileService.FileExists(deletePluginFolderFilePath))
        {
            return;
        }

        var pluginsToDelete = fileService.ReadAllLines(deletePluginFolderFilePath);
        var shouldRemoveMarkedPluginsFile = true;

        foreach (var pluginToDelete in pluginsToDelete)
        {
            logger.LogInformation("Deleting plugin: {PluginToDelete}", pluginToDelete);

            try
            {
                fileService.DeleteDirectory(pluginToDelete);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error deleting plugin '{PluginToDelete};", pluginToDelete);
                shouldRemoveMarkedPluginsFile = false;
            }
        }

        if (shouldRemoveMarkedPluginsFile)
        {
            fileService.DeleteFile(deletePluginFolderFilePath);
        }
    }

    private string GetDeletePluginFolderFilePath()
    {
        return fileService.CombinePaths(ConfigFolders.PathToPlugins, DeletePluginFolderFileName);
    }
}

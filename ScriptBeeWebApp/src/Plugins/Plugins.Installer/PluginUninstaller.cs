using Microsoft.Extensions.Logging;
using ScriptBee.Common.Extensions;

namespace ScriptBee.Plugins.Installer;

public class PluginUninstaller(
    IPluginPathProvider pluginPathProvider,
    ILogger<PluginUninstaller> logger
) : IPluginUninstaller
{
    private const string DeletePluginFolderFileName = "__delete.txt";

    public void ForceUninstall(string pathToPlugin)
    {
        logger.LogInformation("Force uninstalling plugin from {PathToPlugin}", pathToPlugin);
        new DirectoryInfo(pathToPlugin).DeleteIfExists();
    }

    public void Uninstall(string pathToPlugin)
    {
        logger.LogInformation("Marking plugin for deletion: {PathToPlugin}", pathToPlugin);

        var deletePluginFolderFilePath = GetDeletePluginFolderFilePath();

        File.AppendAllLines(deletePluginFolderFilePath, new[] { pathToPlugin });
    }

    public void DeleteMarkedPlugins()
    {
        var deletePluginFolderFilePath = GetDeletePluginFolderFilePath();

        if (!File.Exists(deletePluginFolderFilePath))
        {
            return;
        }

        var pluginsToDelete = File.ReadAllLines(deletePluginFolderFilePath);
        var shouldRemoveMarkedPluginsFile = true;

        foreach (var pluginToDelete in pluginsToDelete)
        {
            logger.LogInformation("Deleting plugin: {PluginToDelete}", pluginToDelete);

            try
            {
                new DirectoryInfo(pluginToDelete).DeleteIfExists();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error deleting plugin '{PluginToDelete}';", pluginToDelete);
                shouldRemoveMarkedPluginsFile = false;
            }
        }

        if (shouldRemoveMarkedPluginsFile)
        {
            File.Delete(deletePluginFolderFilePath);
        }
    }

    private string GetDeletePluginFolderFilePath()
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        return Path.Combine(pluginFolderPath, DeletePluginFolderFileName);
    }
}

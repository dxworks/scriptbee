using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Installer;

public class SimplePluginInstaller(
    IFileService fileService,
    IZipFileService zipFileService,
    IDownloadService downloadService,
    IPluginPathProvider pluginPathProvider,
    ILogger<SimplePluginInstaller> logger
) : ISimplePluginInstaller
{
    public async Task<OneOf<string, PluginInstallationError>> Install(
        string url,
        PluginId pluginId,
        CancellationToken cancellationToken = default
    )
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        fileService.CreateFolder(pluginFolderPath);

        var pluginPath = GetPluginFolderPath(pluginId);
        var zipFilePath = $"{pluginPath}.zip";

        if (fileService.DirectoryExists(pluginPath))
        {
            return pluginPath;
        }

        try
        {
            await downloadService.DownloadFileAsync(url, zipFilePath, cancellationToken);

            await zipFileService.UnzipFileAsync(zipFilePath, pluginPath, cancellationToken);

            return pluginPath;
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Error while downloading plugin {Name} {Version}",
                pluginId.Name,
                pluginId.Version
            );

            fileService.DeleteDirectory(pluginPath);
            return new PluginInstallationError(pluginId, []);
        }
        finally
        {
            fileService.DeleteFile(zipFilePath);
        }
    }

    private string GetPluginFolderPath(PluginId pluginId)
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        return fileService.CombinePaths(pluginFolderPath, pluginId.GetFullyQualifiedName());
    }
}

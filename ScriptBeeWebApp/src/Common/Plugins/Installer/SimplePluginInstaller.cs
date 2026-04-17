using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Artifacts;

namespace ScriptBee.Plugins.Installer;

public class SimplePluginInstaller(
    IFileService fileService,
    IZipFileService zipFileService,
    IDownloadService downloadService,
    IPluginPathProvider pluginPathProvider,
    ILogger<SimplePluginInstaller> logger
) : ISimplePluginInstaller
{
    public async Task<OneOf<string, PluginVersionExistsError, PluginInstallationError>> Install(
        string url,
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    )
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        fileService.CreateFolder(pluginFolderPath);

        var pluginPath = GetPluginFolderPath(pluginId, version);
        var zipFilePath = $"{pluginPath}.zip";

        if (fileService.DirectoryExists(pluginPath))
        {
            return new PluginVersionExistsError(pluginId, version);
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
                pluginId,
                version
            );

            fileService.DeleteDirectory(pluginPath);
            return new PluginInstallationError(pluginId, version);
        }
        finally
        {
            fileService.DeleteFile(zipFilePath);
        }
    }

    private string GetPluginFolderPath(string name, string version)
    {
        var pluginName = PluginNameGenerator.GetPluginName(name, version);
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        return fileService.CombinePaths(pluginFolderPath, pluginName);
    }
}

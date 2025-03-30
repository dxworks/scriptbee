using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Persistence.File.Exceptions;

namespace ScriptBee.Persistence.File.Plugin.Installer;

public class SimplePluginInstaller(
    IFileService fileService,
    IZipFileService zipFileService,
    IDownloadService downloadService,
    ILogger<SimplePluginInstaller> logger
) : ISimplePluginInstaller
{
    public async Task<string> Install(
        string url,
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    )
    {
        fileService.CreateFolder(ConfigFolders.PathToPlugins);

        var pluginPath = GetPluginFolderPath(pluginId, version);
        var zipFilePath = $"{pluginPath}.zip";

        if (fileService.DirectoryExists(pluginPath))
        {
            // TODO FIXIT(#51): convert to error instead of exception
            throw new PluginVersionExistsException(pluginId, version);
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

            // TODO FIXIT(#51): convert to error instead of exception
            throw new PluginInstallationException(pluginId, version);
        }
        finally
        {
            fileService.DeleteFile(zipFilePath);
        }
    }

    private string GetPluginFolderPath(string name, string version)
    {
        var pluginName = PluginNameGenerator.GetPluginName(name, version);
        return fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);
    }
}

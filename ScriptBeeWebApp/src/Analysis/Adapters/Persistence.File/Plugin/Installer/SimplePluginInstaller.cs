using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Ports.Plugins.Installer;

namespace ScriptBee.Persistence.File.Plugin.Installer;

public class SimplePluginInstaller(
    IFileService fileService,
    IZipFileService zipFileService,
    IDownloadService downloadService,
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
        fileService.CreateFolder(ConfigFolders.PathToPlugins);

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
        return fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);
    }
}

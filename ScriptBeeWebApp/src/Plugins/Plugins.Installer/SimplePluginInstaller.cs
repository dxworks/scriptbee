using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Common.Extensions;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Installer;

public class SimplePluginInstaller(
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
        pluginFolderPath.EnsureDirectoryExists();

        var pluginPath = GetPluginFolderPath(pluginId);
        var zipFilePath = $"{pluginPath}.zip";

        if (Directory.Exists(pluginPath))
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
                "Error installing plugin {Name} with {Version}",
                pluginId.Name,
                pluginId.Version
            );

            new DirectoryInfo(pluginPath).DeleteIfExists();

            return new PluginInstallationError(pluginId, []);
        }
        finally
        {
            new FileInfo(zipFilePath).DeleteIfExists();
        }
    }

    private string GetPluginFolderPath(PluginId pluginId)
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        return Path.Combine(pluginFolderPath, pluginId.GetFullyQualifiedName());
    }
}

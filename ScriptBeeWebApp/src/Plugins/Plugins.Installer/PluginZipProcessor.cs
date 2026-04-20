using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Common.Extensions;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Plugins.Installer;

public class PluginZipProcessor(
    IZipFileService zipFileService,
    IPluginReader pluginReader,
    IPluginPathProvider pluginPathProvider,
    ILogger<PluginZipProcessor> logger
) : IPluginZipProcessor
{
    public async Task<
        OneOf<
            PluginId,
            PluginManifestNotFoundError,
            PluginAlreadyExistsError,
            PluginInstallationError
        >
    > ProcessZipStream(ProjectId projectId, Stream zipStream, CancellationToken cancellationToken)
    {
        var projectPluginsPath = pluginPathProvider.GetPathToPlugins(projectId);
        projectPluginsPath.EnsureDirectoryExists();

        var tempZipPath = Path.Combine(projectPluginsPath, $"{Guid.NewGuid()}.zip");
        var tempFolderPath = Path.Combine(projectPluginsPath, Guid.NewGuid().ToString());

        try
        {
            await using (var fileStream = File.Create(tempZipPath))
            {
                await zipStream.CopyToAsync(fileStream, cancellationToken);
            }

            await zipFileService.UnzipFileAsync(tempZipPath, tempFolderPath, cancellationToken);

            var plugin = pluginReader.ReadPlugin(tempFolderPath);

            if (plugin is null)
            {
                return new PluginManifestNotFoundError();
            }

            var finalPluginPath = GetProjectPluginFolderPath(projectId, plugin.Id);

            if (
                Directory.Exists(finalPluginPath)
                || Directory.Exists(GetGlobalPluginFolderPath(plugin.Id))
            )
            {
                return new PluginAlreadyExistsError(plugin.Id);
            }

            Directory.Move(tempFolderPath, finalPluginPath);

            return plugin.Id;
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Error processing uploaded plugin zip for project {ProjectId}: {ErrorMessage}",
                projectId,
                e.Message
            );
            return new PluginInstallationError(new PluginId("Unknown", new Version("0.0.0")), []);
        }
        finally
        {
            new FileInfo(tempZipPath).DeleteIfExists();
            new DirectoryInfo(tempFolderPath).DeleteIfExists();
        }
    }

    private string GetProjectPluginFolderPath(ProjectId projectId, PluginId pluginId)
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins(projectId);
        return Path.Combine(pluginFolderPath, pluginId.GetFullyQualifiedName());
    }

    private string GetGlobalPluginFolderPath(PluginId pluginId)
    {
        var pluginFolderPath = pluginPathProvider.GetPathToPlugins();
        return Path.Combine(pluginFolderPath, pluginId.GetFullyQualifiedName());
    }
}

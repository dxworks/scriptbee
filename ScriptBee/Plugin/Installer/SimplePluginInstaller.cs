using System;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Exceptions;
using ScriptBee.Services;
using Serilog;

namespace ScriptBee.Plugin.Installer;

public class SimplePluginInstaller : ISimplePluginInstaller
{
    private readonly IFileService _fileService;
    private readonly IZipFileService _zipFileService;
    private readonly IDownloadService _downloadService;
    private readonly ILogger _logger;

    public SimplePluginInstaller(IFileService fileService, IZipFileService zipFileService,
        IDownloadService downloadService,
        ILogger logger)
    {
        _fileService = fileService;
        _zipFileService = zipFileService;
        _downloadService = downloadService;
        _logger = logger;
    }

    public async Task<string> Install(string url, string pluginId, string version,
        CancellationToken cancellationToken = default)
    {
        _fileService.CreateFolder(ConfigFolders.PathToPlugins);

        var pluginPath = GetPluginFolderPath(pluginId, version);
        var zipFilePath = $"{pluginPath}.zip";

        if (_fileService.DirectoryExists(pluginPath))
        {
            throw new PluginVersionExistsException(pluginId, version);
        }

        try
        {
            await _downloadService.DownloadFileAsync(url, zipFilePath, cancellationToken);

            await _zipFileService.UnzipFileAsync(zipFilePath, pluginPath, cancellationToken);

            return pluginPath;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while downloading plugin {Name} {Version}", pluginId, version);

            _fileService.DeleteDirectory(pluginPath);

            throw new PluginInstallationException(pluginId, version);
        }
        finally
        {
            _fileService.DeleteFile(zipFilePath);
        }
    }

    private string GetPluginFolderPath(string name, string version)
    {
        var pluginName = PluginNameGenerator.GetPluginName(name, version);
        return _fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);
    }
}

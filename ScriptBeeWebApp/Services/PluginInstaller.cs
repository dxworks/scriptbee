using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Marketplace.Client.Repository;
using ScriptBee.Marketplace.Client.Services;
using ScriptBee.Plugin;

namespace ScriptBeeWebApp.Services;

public class PluginInstaller : IPluginInstaller
{
    private readonly IStorageRepository _storageRepository;
    private readonly IPluginFetcher _pluginFetcher;
    private readonly IDownloadService _downloadService;
    private readonly IFileService _fileService;
    private readonly IZipService _zipService;

    public PluginInstaller(IStorageRepository storageRepository, IPluginFetcher pluginFetcher,
        IDownloadService downloadService, IFileService fileService, IZipService zipService)
    {
        _storageRepository = storageRepository;
        _pluginFetcher = pluginFetcher;
        _downloadService = downloadService;
        _fileService = fileService;
        _zipService = zipService;
    }

    public async Task InstallPlugin(string pluginId, string version, CancellationToken cancellationToken = default)
    {
        var plugin = await _pluginFetcher.GetPluginAsync(pluginId, cancellationToken);

        var pluginVersion = plugin.Versions.FirstOrDefault(v => v.Version == version);

        if (pluginVersion is null)
        {
            // todo add custom exception
            throw new Exception($"Plugin {pluginId} does not have version {version}");
        }

        var pluginName = PluginNameGenerator.GetPluginName(pluginId, pluginVersion.Version);
        var pluginFolder = _fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);
        var zipPath = _fileService.CombinePaths(ConfigFolders.PathToPlugins, $"{pluginName}.zip");

        var pluginVersionUrl = await _storageRepository.GetDownloadUrlAsync(pluginVersion.Url);
        await _downloadService.DownloadFileAsync(pluginVersionUrl, zipPath, cancellationToken);

        await _zipService.UnzipFileAsync(zipPath, pluginFolder, cancellationToken);

        _fileService.DeleteFile(zipPath);
    }

    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        // todo: remove the hard coded file\
        var deleteFolders = _fileService.CombinePaths(ConfigFolders.PathToPlugins, "delete.txt");
        var pluginName = PluginNameGenerator.GetPluginName(pluginId, pluginVersion);
        var pluginFolder = _fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);
        
        _fileService.AppendTextToFile(deleteFolders, pluginFolder);
    }
}

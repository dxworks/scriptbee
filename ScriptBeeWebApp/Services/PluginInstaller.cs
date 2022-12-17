using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Marketplace.Client.Services;
using ScriptBee.Plugin;

namespace ScriptBeeWebApp.Services;

public class PluginInstaller : IPluginInstaller
{
    private readonly IMarketPluginFetcher _marketPluginFetcher;
    private readonly IDownloadService _downloadService;
    private readonly IFileService _fileService;
    private readonly IZipService _zipService;

    public PluginInstaller(IMarketPluginFetcher marketPluginFetcher, IDownloadService downloadService,
        IFileService fileService,
        IZipService zipService)
    {
        _marketPluginFetcher = marketPluginFetcher;
        _downloadService = downloadService;
        _fileService = fileService;
        _zipService = zipService;
    }

    // todo refactor this to include the plugin repository
    public async Task<string> InstallPlugin(string pluginId, string version,
        CancellationToken cancellationToken = default)
    {
        var plugins = await _marketPluginFetcher.GetProjectsAsync(cancellationToken);

        var plugin = plugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin is null)
        {
            // todo add custom exception
            throw new Exception("Plugin not found");
        }

        var semver = Version.Parse(version);
        var pluginVersion = plugin.Versions.FirstOrDefault(v => v.Version == semver);

        if (pluginVersion is null)
        {
            // todo add custom exception
            throw new Exception($"Plugin {pluginId} does not have version {version}");
        }

        var pluginName = PluginNameGenerator.GetPluginName(pluginId, pluginVersion.Version);
        var pluginFolder = _fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);
        var zipPath = _fileService.CombinePaths(ConfigFolders.PathToPlugins, $"{pluginName}.zip");

        var pluginVersionUrl = pluginVersion.Url;

        await _downloadService.DownloadFileAsync(pluginVersionUrl, zipPath, cancellationToken);

        await _zipService.UnzipFileAsync(zipPath, pluginFolder, cancellationToken);

        _fileService.DeleteFile(zipPath);

        return pluginFolder;
    }

    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        // todo: remove the hard coded file
        var deleteFolders = _fileService.CombinePaths(ConfigFolders.PathToPlugins, "delete.txt");
        var pluginName = PluginNameGenerator.GetPluginName(pluginId, pluginVersion);
        var pluginFolder = _fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginName);

        _fileService.AppendTextToFile(deleteFolders, pluginFolder);
    }
}

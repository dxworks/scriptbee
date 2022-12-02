using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Marketplace.Client.Repository;
using ScriptBee.Marketplace.Client.Services;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers.DTO;

namespace ScriptBeeWebApp.Services;

// todo add tests
public sealed class PluginService : IPluginService
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IStorageRepository _storageRepository;
    private readonly IPluginFetcher _pluginFetcher;
    private readonly IDownloadService _downloadService;
    private readonly IFileService _fileService;
    private readonly IZipService _zipService;

    public PluginService(IPluginRepository pluginRepository, IStorageRepository storageRepository,
        IPluginFetcher pluginFetcher, IDownloadService downloadService, IFileService fileService,
        IZipService zipService)
    {
        _pluginRepository = pluginRepository;
        _storageRepository = storageRepository;
        _pluginFetcher = pluginFetcher;
        _downloadService = downloadService;
        _fileService = fileService;
        _zipService = zipService;
    }

    public IEnumerable<PluginManifest> GetPluginManifests()
    {
        return _pluginRepository.GetLoadedPluginManifests();
    }

    public IEnumerable<PluginManifest> GetPluginManifests(string type)
    {
        return _pluginRepository.GetLoadedPluginManifests().Where(manifest =>
            manifest.ExtensionPoints.Any(extensionPoint => extensionPoint.Kind == type));
    }

    public IEnumerable<T> GetExtensionPoints<T>() where T : PluginExtensionPoint
    {
        return _pluginRepository.GetLoadedPluginExtensionPoints<T>();
    }

    public async Task<IEnumerable<MarketplacePlugin>> GetMarketPlugins(int start, int count,
        CancellationToken cancellationToken = default)
    {
        var plugins = await _pluginFetcher.GetPluginsAsync(cancellationToken);

        return plugins.Select(plugin =>
        {
            var pluginVersions = new Dictionary<string, PluginVersion>();

            foreach (var (_, pluginVersion, extensionPointVersions) in plugin.Versions)
            {
                var versions = extensionPointVersions
                    .Select(extensionPointVersion =>
                        new ExtensionPointVersion(extensionPointVersion.Kind, extensionPointVersion.Version))
                    .ToList();

                // todo set installed bool
                pluginVersions.Add(pluginVersion, new PluginVersion(versions, false));
            }


            return new MarketplacePlugin(plugin.Name, plugin.Name, plugin.Author, plugin.Description, pluginVersions);
        });

        // for (var i = start; i < start + count; i++)
        // {
        //     yield return new MarketplaceBundlePlugin(
        //         i.ToString(),
        //         $"Bundle {i}",
        //         $"Author {i}",
        //         $"Description {i}",
        //         $"DownloadUrl {i}",
        //         new Dictionary<string, BundlePluginVersion>
        //         {
        //             {
        //                 "1.0.0", new BundlePluginVersion(new List<BundlePlugin>
        //                 {
        //                     new("Plugin 1", "1.0.0", new List<string> { "ScriptRunner", "ScriptGenerator" }),
        //                     new("Plugin 2", "1.0.0",
        //                         new List<string> { "ScriptRunner", "ScriptGenerator", "HelperFunctions" }),
        //                 }, true)
        //             },
        //             {
        //                 "1.0.1", new BundlePluginVersion(new List<BundlePlugin>
        //                 {
        //                     new("Plugin 1", "1.0.1", new List<string> { "ScriptRunner", "ScriptGenerator" }),
        //                     new("Plugin 2", "1.0.1",
        //                         new List<string> { "ScriptRunner", "ScriptGenerator", "HelperFunctions" }),
        //                     new("Plugin 3", "1.0.1",
        //                         new List<string> { "ScriptRunner", "ScriptGenerator", "HelperFunctions" }),
        //                 }, false)
        //             },
        //         }
        //     );
        // }
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

        var pluginFolder = _fileService.CombinePaths(ConfigFolders.PathToPlugins, pluginId);
        var zipPath = _fileService.CombinePaths(ConfigFolders.PathToPlugins, $"{pluginId}.zip");

        _fileService.DeleteFolder(pluginFolder);

        var pluginVersionUrl = await _storageRepository.GetDownloadUrlAsync(pluginVersion.Url);
        await _downloadService.DownloadFileAsync(pluginVersionUrl, zipPath, cancellationToken);

        await _zipService.UnzipFileAsync(zipPath, pluginFolder, cancellationToken);

        _fileService.DeleteFile(zipPath);
    }

    public Task UninstallPlugin(string pluginId)
    {
        throw new NotImplementedException();
    }
}

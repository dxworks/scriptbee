using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Marketplace.Client.Services;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers.DTO;

namespace ScriptBeeWebApp.Services;

// todo add tests
public sealed class PluginService : IPluginService
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IPluginInstaller _pluginInstaller;
    private readonly IMarketPluginFetcher _marketPluginFetcher;
    private readonly IPluginReader _pluginReader;
    private readonly IPluginLoader _pluginLoader;

    public PluginService(IPluginRepository pluginRepository, IPluginInstaller pluginInstaller,
        IMarketPluginFetcher marketPluginFetcher, IPluginReader pluginReader, IPluginLoader pluginLoader)
    {
        _pluginRepository = pluginRepository;
        _pluginInstaller = pluginInstaller;
        _marketPluginFetcher = marketPluginFetcher;
        _pluginReader = pluginReader;
        _pluginLoader = pluginLoader;
    }

    public IEnumerable<PluginManifest> GetPluginManifests()
    {
        return _pluginRepository.GetLoadedPluginsManifests();
    }

    public IEnumerable<PluginManifest> GetPluginManifests(string type)
    {
        return _pluginRepository.GetLoadedPlugins(type)
            .Select(plugin => plugin.Manifest);
    }

    public IEnumerable<T> GetExtensionPoints<T>() where T : PluginExtensionPoint
    {
        return _pluginRepository.GetLoadedPluginExtensionPoints<T>();
    }

    public async Task<IEnumerable<MarketplacePlugin>> GetMarketPlugins(int start, int count,
        CancellationToken cancellationToken = default)
    {
        var plugins = await _marketPluginFetcher.GetPluginsAsync(cancellationToken);

        return plugins.Select(plugin =>
        {
            var pluginVersions = new List<PluginVersion>();

            foreach (var (_, pluginVersion, extensionPointVersions) in plugin.Versions)
            {
                var versions = extensionPointVersions
                    .Select(extensionPointVersion =>
                        new ExtensionPointVersion(extensionPointVersion.Kind, extensionPointVersion.Version.ToString()))
                    .ToList();

                var installedPluginVersion = _pluginRepository.GetInstalledPluginVersion(plugin.Id);

                var installed = installedPluginVersion is not null &&
                                installedPluginVersion.CompareTo(pluginVersion) == 0;
                pluginVersions.Add(new PluginVersion(pluginVersion.ToString(), versions, installed));
            }

            return new MarketplacePlugin(plugin.Id, plugin.Name, plugin.Description, plugin.Authors, pluginVersions);
        });
    }

    public async Task InstallPlugin(string pluginId, string version, CancellationToken cancellationToken = default)
    {
        var installedPluginPath = await _pluginInstaller.InstallPlugin(pluginId, version, cancellationToken);

        var plugin = _pluginReader.ReadPlugin(installedPluginPath);
        if (plugin is not null)
        {
            _pluginLoader.Load(plugin);
        }
    }

    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        _pluginRepository.UnRegisterPlugin(pluginId, pluginId);
        _pluginInstaller.UninstallPlugin(pluginId, pluginVersion);
    }
}

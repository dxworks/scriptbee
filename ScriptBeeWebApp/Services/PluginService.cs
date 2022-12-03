using System;
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
    private readonly IPluginFetcher _pluginFetcher;

    public PluginService(IPluginRepository pluginRepository, IPluginInstaller pluginInstaller,
        IPluginFetcher pluginFetcher)
    {
        _pluginRepository = pluginRepository;
        _pluginInstaller = pluginInstaller;
        _pluginFetcher = pluginFetcher;
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

                var installedPluginVersion = _pluginRepository.GetInstalledPluginVersion(plugin.Name);

                var installed = installedPluginVersion is not null &&
                                installedPluginVersion.CompareTo(Version.Parse(pluginVersion)) == 0;
                pluginVersions.Add(pluginVersion, new PluginVersion(versions, installed));
            }

            return new MarketplacePlugin(plugin.Name, plugin.Name, plugin.Author, plugin.Description, pluginVersions);
        });
    }

    public async Task InstallPlugin(string pluginId, string version, CancellationToken cancellationToken = default)
    {
        await _pluginInstaller.InstallPlugin(pluginId, version, cancellationToken);
    }

    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        _pluginRepository.UnRegisterPlugin(pluginId, pluginId);
        _pluginInstaller.UninstallPlugin(pluginId, pluginVersion);
    }
}

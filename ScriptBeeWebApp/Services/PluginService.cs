using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Marketplace.Client.Data;
using ScriptBee.Marketplace.Client.Services;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers.DTO;
using PluginVersion = ScriptBeeWebApp.Controllers.DTO.PluginVersion;

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

    public async Task<IEnumerable<MarketplaceProject>> GetMarketPlugins(int start, int count,
        CancellationToken cancellationToken = default)
    {
        var projects = await _marketPluginFetcher.GetProjectsAsync(cancellationToken);

        return projects
            .Select(plugin =>
            {
                var pluginVersions = plugin.Versions
                    .Select(pluginVersion => GetPluginVersion(pluginVersion, plugin))
                    .ToList();
                var type = plugin.Type == MarketPlaceProjectType.Plugin
                    ? MarketplaceProject.PluginType
                    : MarketplaceProject.BundleType;

                return new MarketplaceProject(plugin.Id, plugin.Name, type, plugin.Description,
                    plugin.Authors, pluginVersions);
            });
    }

    public async Task InstallPlugin(string pluginId, string version, CancellationToken cancellationToken = default)
    {
        var installedPluginPath = await _pluginInstaller.InstallPlugin(pluginId, version, cancellationToken);

        var plugin = _pluginReader.ReadPlugin(installedPluginPath);
        if (plugin is not null)
        {
            var installPluginTasks = new List<Task>();

            foreach (var extensionPoint in plugin.Manifest.ExtensionPoints.Where(point =>
                         point.Kind == PluginKind.Plugin))
            {
                installPluginTasks.Add(InstallPlugin(extensionPoint.EntryPoint, extensionPoint.Version,
                    cancellationToken));
            }

            await Task.WhenAll(installPluginTasks);

            _pluginLoader.Load(plugin);
        }
    }

    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        _pluginRepository.UnRegisterPlugin(pluginId, pluginId);
        _pluginInstaller.UninstallPlugin(pluginId, pluginVersion);
    }

    private PluginVersion GetPluginVersion(ScriptBee.Marketplace.Client.Data.PluginVersion pluginVersion,
        MarketPlaceProject plugin)
    {
        var (_, version, _) = pluginVersion;
        var installedPluginVersion = _pluginRepository.GetInstalledPluginVersion(plugin.Id);

        var installed = installedPluginVersion is not null &&
                        installedPluginVersion.CompareTo(version) == 0;

        return new PluginVersion(version.ToString(), installed);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Marketplace.Client.Data;
using ScriptBee.Marketplace.Client.Services;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Installer;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers.DTO;
using Serilog;
using PluginVersion = ScriptBeeWebApp.Controllers.DTO.PluginVersion;

namespace ScriptBeeWebApp.Services;

// todo add tests
public sealed class PluginService : IPluginService
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IBundlePluginInstaller _bundlePluginInstaller;
    private readonly IBundlePluginUninstaller _bundlePluginUninstaller;
    private readonly IMarketPluginFetcher _marketPluginFetcher;
    private readonly IPluginReader _pluginReader;
    private readonly IPluginLoader _pluginLoader;
    private readonly ILogger _logger;

    public PluginService(IPluginRepository pluginRepository, IBundlePluginInstaller bundlePluginInstaller,
        IBundlePluginUninstaller bundlePluginUninstaller, IMarketPluginFetcher marketPluginFetcher,
        IPluginReader pluginReader, IPluginLoader pluginLoader, ILogger logger)
    {
        _pluginRepository = pluginRepository;
        _bundlePluginInstaller = bundlePluginInstaller;
        _bundlePluginUninstaller = bundlePluginUninstaller;
        _marketPluginFetcher = marketPluginFetcher;
        _pluginReader = pluginReader;
        _pluginLoader = pluginLoader;
        _logger = logger;
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

    public async Task<IEnumerable<MarketplaceProject>> GetMarketPlugins(CancellationToken cancellationToken = default)
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
        var installPluginPaths = await _bundlePluginInstaller.Install(pluginId, version, cancellationToken);

        foreach (var installPluginPath in installPluginPaths)
        {
            var plugin = _pluginReader.ReadPlugin(installPluginPath);
            if (plugin is null)
            {
                _logger.Warning("Plugin Manifest from {Path} could not be read", installPluginPath);
                continue;
            }

            _pluginLoader.Load(plugin);
        }
    }

    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        var uninstalledPluginVersions = _bundlePluginUninstaller.Uninstall(pluginId, pluginVersion);

        foreach (var (plugin, version) in uninstalledPluginVersions)
        {
            _pluginRepository.UnRegisterPlugin(plugin, version);
        }
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

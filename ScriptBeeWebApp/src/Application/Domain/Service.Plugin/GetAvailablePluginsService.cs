using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.MarketPlace;
using ScriptBee.Ports.Plugins;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin;

public class GetAvailablePluginsService(
    IMarketPluginFetcher marketPluginFetcher,
    IPluginRepository pluginRepository
) : IGetAvailablePluginsUseCase
{
    public async Task<IEnumerable<MarketPlacePluginEntry>> GetMarketPlugins(
        CancellationToken cancellationToken = default
    )
    {
        await marketPluginFetcher.UpdateRepositoryAsync(cancellationToken);

        var projects = marketPluginFetcher.GetProjectsAsync();

        return projects.Select(plugin =>
        {
            var pluginVersions = plugin.Versions.Select(pluginVersion =>
                GetPluginVersion(pluginVersion, plugin)
            );

            return new MarketPlacePluginEntry(plugin, pluginVersions);
        });
    }

    private MarketPlacePluginVersion GetPluginVersion(
        PluginVersion pluginVersion,
        MarketPlacePlugin plugin
    )
    {
        var (_, version, _) = pluginVersion;
        var installedPluginVersion = pluginRepository.GetInstalledPluginVersion(plugin.Id);

        var installed =
            installedPluginVersion is not null && installedPluginVersion.CompareTo(version) == 0;

        return new MarketPlacePluginVersion(version, installed);
    }
}

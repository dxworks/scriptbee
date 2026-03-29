using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugin.MarketPlace;
using ScriptBee.Marketplace.Client;
using ScriptBee.UseCases.Project.Plugin;

namespace ScriptBee.Service.Project.Plugin;

public class GetAvailablePluginsService(IMarketPluginFetcher marketPluginFetcher)
    : IGetAvailablePluginsUseCase
{
    public async Task<IEnumerable<MarketPlacePlugin>> GetMarketPlugins(
        CancellationToken cancellationToken
    )
    {
        return await marketPluginFetcher.GetProjectsAsync(cancellationToken);
    }

    public async Task<OneOf<MarketPlacePlugin, PluginNotFoundError>> GetMarketPlugin(
        string pluginId,
        CancellationToken cancellationToken
    )
    {
        var plugins = await marketPluginFetcher.GetProjectsAsync(cancellationToken);
        var marketPlacePlugin = plugins.FirstOrDefault(plugin => plugin.Id == pluginId);

        return marketPlacePlugin is null ? new PluginNotFoundError(pluginId) : marketPlacePlugin;
    }
}

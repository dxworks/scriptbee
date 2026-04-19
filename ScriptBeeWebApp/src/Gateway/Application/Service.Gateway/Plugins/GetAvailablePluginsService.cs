using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.MarketPlace;
using ScriptBee.Marketplace.Client;
using ScriptBee.UseCases.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Plugins;

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

        return marketPlacePlugin is null
            ? new PluginNotFoundError(new PluginId(pluginId, new Version()))
            : marketPlacePlugin;
    }
}

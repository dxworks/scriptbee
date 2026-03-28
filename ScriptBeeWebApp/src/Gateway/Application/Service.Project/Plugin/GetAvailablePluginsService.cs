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
        CancellationToken cancellationToken = default
    )
    {
        await marketPluginFetcher.UpdateRepositoryAsync(cancellationToken);

        return marketPluginFetcher.GetProjectsAsync();
    }

    public Task<OneOf<MarketPlacePlugin, PluginNotFoundError>> GetMarketPlugins(
        string pluginId,
        CancellationToken cancellationToken = default
    )
    {
        var marketPlacePlugin = marketPluginFetcher
            .GetProjectsAsync()
            .FirstOrDefault(plugin => plugin.Id == pluginId);

        return marketPlacePlugin is null
            ? Task.FromResult<OneOf<MarketPlacePlugin, PluginNotFoundError>>(
                new PluginNotFoundError(pluginId)
            )
            : Task.FromResult<OneOf<MarketPlacePlugin, PluginNotFoundError>>(marketPlacePlugin);
    }
}

using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.UseCases.Gateway.Plugins;

public interface IGetAvailablePluginsUseCase
{
    Task<IEnumerable<MarketPlacePlugin>> GetMarketPlugins(CancellationToken cancellationToken);

    Task<OneOf<MarketPlacePlugin, PluginNotFoundError>> GetMarketPlugin(
        string pluginId,
        CancellationToken cancellationToken
    );
}

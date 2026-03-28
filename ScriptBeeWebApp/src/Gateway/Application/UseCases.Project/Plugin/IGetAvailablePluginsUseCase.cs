using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.UseCases.Project.Plugin;

public interface IGetAvailablePluginsUseCase
{
    Task<IEnumerable<MarketPlacePlugin>> GetMarketPlugins(
        CancellationToken cancellationToken = default
    );

    Task<OneOf<MarketPlacePlugin, PluginNotFoundError>> GetMarketPlugins(
        string pluginId,
        CancellationToken cancellationToken = default
    );
}

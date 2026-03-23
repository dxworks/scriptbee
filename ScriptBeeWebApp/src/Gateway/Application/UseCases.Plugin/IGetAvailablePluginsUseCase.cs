using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.UseCases.Plugin;

public interface IGetAvailablePluginsUseCase
{
    Task<IEnumerable<MarketPlacePluginEntry>> GetMarketPlugins(
        CancellationToken cancellationToken = default
    );
}

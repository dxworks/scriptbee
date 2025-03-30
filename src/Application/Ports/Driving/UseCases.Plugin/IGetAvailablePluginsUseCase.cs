using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.UseCases.Plugin;

public interface IGetAvailablePluginsUseCase
{
    Task<IEnumerable<MarketPlacePlugin>> GetMarketPlugins(
        CancellationToken cancellationToken = default
    );
}

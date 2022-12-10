using ScriptBee.Marketplace.Client.Data;

namespace ScriptBee.Marketplace.Client.Services;

public interface IMarketPluginFetcher
{
    Task<IEnumerable<MarketPlacePlugin>> GetPluginsAsync(CancellationToken cancellationToken = default);
}

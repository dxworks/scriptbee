using ScriptBee.Marketplace.Client.Data;

namespace ScriptBee.Marketplace.Client.Services;

public interface IMarketPluginFetcher
{
    Task<IEnumerable<MarketPlaceProject>> GetProjectsAsync(CancellationToken cancellationToken = default);
}

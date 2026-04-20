using ScriptBee.Domain.Model.Plugins.MarketPlace;

namespace ScriptBee.Plugins.Marketplace;

public interface IMarketPluginFetcher
{
    Task<IEnumerable<MarketPlacePlugin>> GetProjectsAsync(CancellationToken cancellationToken);
}

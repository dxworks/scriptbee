using ScriptBee.Domain.Model.Plugins.MarketPlace;

namespace ScriptBee.Marketplace.Client;

public interface IMarketPluginFetcher
{
    Task<IEnumerable<MarketPlacePlugin>> GetProjectsAsync(CancellationToken cancellationToken);
}

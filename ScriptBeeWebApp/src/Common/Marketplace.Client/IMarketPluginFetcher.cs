using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Marketplace.Client;

public interface IMarketPluginFetcher
{
    Task<IEnumerable<MarketPlacePlugin>> GetProjectsAsync(CancellationToken cancellationToken);
}

using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Marketplace.Client;

public interface IMarketPluginFetcher
{
    Task UpdateRepositoryAsync(CancellationToken cancellationToken = default);

    IEnumerable<MarketPlacePlugin> GetProjectsAsync();
}

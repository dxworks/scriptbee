using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Marketplace.Client.Services;

public interface IMarketPluginFetcher
{
    Task UpdateRepositoryAsync(CancellationToken cancellationToken = default);

    IEnumerable<MarketPlacePlugin> GetProjectsAsync();
}

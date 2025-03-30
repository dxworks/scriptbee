using ScriptBee.Marketplace.Client.Data;

namespace ScriptBee.Marketplace.Client.Services;

public interface IMarketPluginFetcher
{
    Task UpdateRepositoryAsync(CancellationToken cancellationToken = default);

    IEnumerable<MarketPlaceProject> GetProjectsAsync();
}

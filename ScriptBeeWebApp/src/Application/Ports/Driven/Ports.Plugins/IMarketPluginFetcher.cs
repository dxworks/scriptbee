using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Ports.Plugins;

public interface IMarketPluginFetcher
{
    Task UpdateRepositoryAsync(CancellationToken cancellationToken = default);

    IEnumerable<MarketPlacePlugin> GetProjectsAsync();
}

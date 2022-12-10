using DxWorks.Hub.Sdk;
using DxWorks.Hub.Sdk.Project;
using ScriptBee.Marketplace.Client.Data;

namespace ScriptBee.Marketplace.Client.Services;

public sealed class MarketPluginFetcher : IMarketPluginFetcher
{
    private readonly IDxWorksHubClient _hubClient;

    public MarketPluginFetcher(IDxWorksHubClient hubClient)
    {
        _hubClient = hubClient;
    }

    public async Task<IEnumerable<MarketPlacePlugin>> GetPluginsAsync(CancellationToken cancellationToken = default)
    {
        await _hubClient.UpdateRepositoryAsync(cancellationToken);

        return _hubClient.GetProjects()
            .Where(project => project.ScriptBee is not null)
            .Select(ConvertToPlugin);
    }

    private static MarketPlacePlugin ConvertToPlugin(DxWorksProject project)
    {
        return new MarketPlacePlugin
        (
            project.Slug,
            project.Name,
            project.Description,
            project.Authors.Select(author => author.Name).ToList(),
            project.ScriptBee!.Versions.Select(ConvertToPluginVersion).ToList()
        );
    }

    private static PluginVersion ConvertToPluginVersion(ScriptBeeVersion version)
    {
        return new PluginVersion
        (
            version.Asset,
            version.Version,
            (version.ExtensionPoints ?? new List<ExtensionPoint>())
            .Select(point => new ExtensionPointVersion(point.Kind, point.Version)).ToList()
        );
    }
}

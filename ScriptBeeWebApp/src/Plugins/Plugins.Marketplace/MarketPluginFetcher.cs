using DxWorks.Hub.Sdk.Clients;
using DxWorks.Hub.Sdk.Project;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.MarketPlace;

namespace ScriptBee.Plugins.Marketplace;

public sealed class MarketPluginFetcher(IScriptBeeClient hubClient) : IMarketPluginFetcher
{
    private readonly SemaphoreSlim _updateRepository = new(1, 1);

    public async Task<IEnumerable<MarketPlacePlugin>> GetProjectsAsync(
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _updateRepository.WaitAsync(cancellationToken);
            await hubClient.UpdateRepositoryAsync(cancellationToken);
        }
        finally
        {
            _updateRepository.Release();
        }

        return hubClient.GetScriptBeeProjects().Select(ConvertToPlugin);
    }

    private static MarketPlacePlugin ConvertToPlugin(ScriptBeeProject project)
    {
        var projectType =
            project.Type == ScriptBeeProjectTypes.Bundle
                ? MarketPlacePluginType.Bundle
                : MarketPlacePluginType.Plugin;

        return new MarketPlacePlugin(
            project.Id,
            project.Name,
            projectType,
            project.Description,
            project.Authors.Select(author => author.Name).ToList(),
            project.Versions.Select(ConvertToPluginVersion).ToList()
        );
    }

    private static PluginVersion ConvertToPluginVersion(ScriptBeeProjectVersion version)
    {
        return new PluginVersion(version.DownloadUrl, version.Version, version.Manifest);
    }
}

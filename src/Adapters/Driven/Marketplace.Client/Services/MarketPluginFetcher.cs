using DxWorks.Hub.Sdk.Clients;
using DxWorks.Hub.Sdk.Project;
using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Marketplace.Client.Services;

public sealed class MarketPluginFetcher(IScriptBeeClient hubClient) : IMarketPluginFetcher
{
    public async Task UpdateRepositoryAsync(CancellationToken cancellationToken = default)
    {
        await hubClient.UpdateRepositoryAsync(cancellationToken);
    }

    public IEnumerable<MarketPlacePlugin> GetProjectsAsync()
    {
        return hubClient.GetScriptBeeProjects().Select(ConvertToPlugin);
    }

    private static MarketPlacePlugin ConvertToPlugin(ScriptBeeProject project)
    {
        var projectType =
            project.Type == ScriptBeeProjectTypes.Bundle
                ? MarketPlaceProjectType.Bundle
                : MarketPlaceProjectType.Plugin;

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

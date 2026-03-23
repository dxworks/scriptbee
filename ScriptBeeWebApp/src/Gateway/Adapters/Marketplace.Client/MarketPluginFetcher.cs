using DxWorks.Hub.Sdk.Clients;
using DxWorks.Hub.Sdk.Project;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.MarketPlace;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Marketplace.Client;

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

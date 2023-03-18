using DxWorks.Hub.Sdk.Clients;
using DxWorks.Hub.Sdk.Project;
using ScriptBee.Marketplace.Client.Data;

namespace ScriptBee.Marketplace.Client.Services;

public sealed class MarketPluginFetcher : IMarketPluginFetcher
{
    private readonly IScriptBeeClient _hubClient;


    public MarketPluginFetcher(IScriptBeeClient hubClient)
    {
        _hubClient = hubClient;
    }

    public async Task UpdateRepositoryAsync(CancellationToken cancellationToken = default)
    {
        await _hubClient.UpdateRepositoryAsync(cancellationToken);
    }

    public IEnumerable<MarketPlaceProject> GetProjectsAsync()
    {
        return _hubClient.GetScriptBeeProjects()
            .Select(ConvertToPlugin);
    }

    private static MarketPlaceProject ConvertToPlugin(ScriptBeeProject project)
    {
        var projectType = project.Type == ScriptBeeProjectTypes.Bundle
            ? MarketPlaceProjectType.Bundle
            : MarketPlaceProjectType.Plugin;

        return new MarketPlaceProject
        (
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
        return new PluginVersion
        (
            version.DownloadUrl,
            version.Version,
            version.Manifest
        );
    }
}

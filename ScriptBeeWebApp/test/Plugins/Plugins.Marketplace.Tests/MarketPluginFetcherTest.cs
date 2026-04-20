using DxWorks.Hub.Sdk.Clients;
using DxWorks.Hub.Sdk.Project;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins.MarketPlace;

namespace ScriptBee.Plugins.Marketplace.Tests;

public class MarketPluginFetcherTest
{
    private readonly IScriptBeeClient _hubClient = Substitute.For<IScriptBeeClient>();

    private readonly ILogger<MarketPluginFetcher> _logger = Substitute.For<
        ILogger<MarketPluginFetcher>
    >();

    private readonly MarketPluginFetcher _pluginFetcher;

    public MarketPluginFetcherTest()
    {
        _pluginFetcher = new MarketPluginFetcher(_hubClient, _logger);
    }

    [Fact]
    public async Task GetProjectsAsync_CallsHubClientGetScriptBeeProjects()
    {
        await _pluginFetcher.GetProjectsAsync(TestContext.Current.CancellationToken);

        _hubClient.Received(1).GetScriptBeeProjects();
        await _hubClient.Received(1).UpdateRepositoryAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProjectsAsync_ReturnsEmptyList_WhenHubClientReturnsEmptyList()
    {
        _hubClient.GetScriptBeeProjects().Returns([]);

        var result = await _pluginFetcher.GetProjectsAsync(TestContext.Current.CancellationToken);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetProjectsAsync_MapsScriptBeeProjectsToMarketPlaceProjects()
    {
        var scriptBeeProjects = new List<ScriptBeeProject>
        {
            new()
            {
                Id = "project1",
                Name = "Test Plugin",
                Type = ScriptBeeProjectTypes.Plugin,
                Description = "A test plugin description.",
                Authors = [new Author { Name = "John Doe" }],
                Versions =
                [
                    new ScriptBeeProjectVersion
                    {
                        DownloadUrl = "url1",
                        Version = new Version("1.0.0"),
                        Manifest = "manifest1",
                    },
                ],
            },
            new()
            {
                Id = "bundle1",
                Name = "Test Bundle",
                Type = ScriptBeeProjectTypes.Bundle,
                Description = "A test bundle description.",
                Authors = [new Author { Name = "Jane Doe" }],
                Versions =
                [
                    new ScriptBeeProjectVersion
                    {
                        DownloadUrl = "url2",
                        Version = new Version("2.0.0"),
                        Manifest = "manifest2",
                    },
                ],
            },
        };
        _hubClient.GetScriptBeeProjects().Returns(scriptBeeProjects);

        var result = (
            await _pluginFetcher.GetProjectsAsync(TestContext.Current.CancellationToken)
        ).ToList();

        result.Count.ShouldBe(2);
        var pluginProject = result.First(p => p.Id == "project1");
        pluginProject.Name.ShouldBe("Test Plugin");
        pluginProject.Type.ShouldBe(MarketPlacePluginType.Plugin);
        pluginProject.Description.ShouldBe("A test plugin description.");
        pluginProject.Authors.ShouldContain("John Doe");
        var pluginVersion = pluginProject.Versions.Single();
        pluginVersion.Url.ShouldBe("url1");
        pluginVersion.Version.ShouldBe(new Version("1.0.0"));
        pluginVersion.ManifestUrl.ShouldBe("manifest1");

        var bundleProject = result.First(p => p.Id == "bundle1");
        bundleProject.Name.ShouldBe("Test Bundle");
        bundleProject.Type.ShouldBe(MarketPlacePluginType.Bundle);
        bundleProject.Description.ShouldBe("A test bundle description.");
        bundleProject.Authors.ShouldContain("Jane Doe");
        var version = bundleProject.Versions.Single();
        version.Url.ShouldBe("url2");
        version.Version.ShouldBe(new Version("2.0.0"));
        version.ManifestUrl.ShouldBe("manifest2");
    }
}

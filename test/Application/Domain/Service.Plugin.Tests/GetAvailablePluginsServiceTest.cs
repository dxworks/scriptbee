using NSubstitute;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.MarketPlace;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Service.Plugin.Tests;

public class GetAvailablePluginsServiceTest
{
    private readonly IMarketPluginFetcher _marketPluginFetcher =
        Substitute.For<IMarketPluginFetcher>();

    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();
    private readonly GetAvailablePluginsService _getAvailablePluginsService;

    public GetAvailablePluginsServiceTest()
    {
        _getAvailablePluginsService = new GetAvailablePluginsService(
            _marketPluginFetcher,
            _pluginRepository
        );
    }

    [Fact]
    public async Task GetMarketPlugins_CallsUpdateRepositoryAsync()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        await _getAvailablePluginsService.GetMarketPlugins(cancellationToken);

        await _marketPluginFetcher.Received(1).UpdateRepositoryAsync(cancellationToken);
    }

    [Fact]
    public async Task GetMarketPlugins_GetsProjectsFromFetcher()
    {
        await _getAvailablePluginsService.GetMarketPlugins(TestContext.Current.CancellationToken);

        _marketPluginFetcher.Received(1).GetProjectsAsync();
    }

    [Fact]
    public async Task GetMarketPlugins_ReturnsEmptyList_WhenFetcherReturnsEmptyList()
    {
        _marketPluginFetcher.GetProjectsAsync().Returns([]);

        var result = await _getAvailablePluginsService.GetMarketPlugins(
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetMarketPlugins_MapsProjectsToMarketPlacePluginEntries()
    {
        // Arrange
        var marketPlacePlugins = new List<MarketPlacePlugin>
        {
            new(
                "id1",
                "Plugin 1",
                MarketPlacePluginType.Plugin,
                "Desc 1",
                [],
                [new PluginVersion("url1", new Version("1.0.0"), "manifest1")]
            ),
            new(
                "id2",
                "Bundle 2",
                MarketPlacePluginType.Bundle,
                "Desc 2",
                [],
                [
                    new PluginVersion("url2", new Version("2.0.0"), "manifest2"),
                    new PluginVersion("url3", new Version("1.5.0"), "manifest3"),
                ]
            ),
        };
        _marketPluginFetcher.GetProjectsAsync().Returns(marketPlacePlugins);
        _pluginRepository.GetInstalledPluginVersion("id1").Returns(new Version("1.0.0"));
        _pluginRepository.GetInstalledPluginVersion("id2").Returns((Version?)null);

        // Act
        var result = (
            await _getAvailablePluginsService.GetMarketPlugins(
                TestContext.Current.CancellationToken
            )
        ).ToList();

        // Assert
        result.Count.ShouldBe(2);
        var plugin1Entry = result.First(p => p.Plugin.Id == "id1");
        plugin1Entry.Plugin.Name.ShouldBe("Plugin 1");
        var plugin1EntryInstalledVersion = plugin1Entry.InstalledVersions.Single();
        plugin1EntryInstalledVersion.Version.ShouldBe(new Version("1.0.0"));
        plugin1EntryInstalledVersion.Installed.ShouldBeTrue();

        var bundle2Entry = result.First(p => p.Plugin.Id == "id2");
        bundle2Entry.Plugin.Name.ShouldBe("Bundle 2");
        bundle2Entry.InstalledVersions.Count().ShouldBe(2);
        bundle2Entry.InstalledVersions.ShouldContain(v =>
            v.Version == new Version("2.0.0") && !v.Installed
        );
        bundle2Entry.InstalledVersions.ShouldContain(v =>
            v.Version == new Version("1.5.0") && !v.Installed
        );
    }

    [Fact]
    public async Task GetMarketPlugins_SetsInstalledToTrue_WhenInstalledVersionMatches()
    {
        var marketPlacePlugin = new MarketPlacePlugin(
            "testId",
            "Test Plugin",
            MarketPlacePluginType.Plugin,
            "Test Description",
            [],
            [new PluginVersion("testUrl", new Version("2.1.0"), "testManifest")]
        );
        _marketPluginFetcher
            .GetProjectsAsync()
            .Returns(new List<MarketPlacePlugin> { marketPlacePlugin });
        _pluginRepository.GetInstalledPluginVersion("testId").Returns(new Version("2.1.0"));

        var result = (
            await _getAvailablePluginsService.GetMarketPlugins(
                TestContext.Current.CancellationToken
            )
        ).Single();

        result.InstalledVersions.Single().Installed.ShouldBeTrue();
    }

    [Fact]
    public async Task GetMarketPlugins_SetsInstalledToFalse_WhenNoInstalledVersion()
    {
        var marketPlacePlugin = new MarketPlacePlugin(
            "testId",
            "Test Plugin",
            MarketPlacePluginType.Plugin,
            "Test Description",
            [],
            [new PluginVersion("testUrl", new Version("1.00"), "testManifest")]
        );
        _marketPluginFetcher
            .GetProjectsAsync()
            .Returns(new List<MarketPlacePlugin> { marketPlacePlugin });
        _pluginRepository.GetInstalledPluginVersion("testId").Returns((Version?)null);

        var result = (
            await _getAvailablePluginsService.GetMarketPlugins(
                TestContext.Current.CancellationToken
            )
        ).Single();

        result.InstalledVersions.Single().Installed.ShouldBeFalse();
    }

    [Fact]
    public async Task GetMarketPlugins_SetsInstalledToFalse_WhenInstalledVersionDoesNotMatch()
    {
        var marketPlacePlugin = new MarketPlacePlugin(
            "testId",
            "Test Plugin",
            MarketPlacePluginType.Plugin,
            "Test Description",
            [],
            [new PluginVersion("testUrl", new Version("1.5.0"), "testManifest")]
        );
        _marketPluginFetcher
            .GetProjectsAsync()
            .Returns(new List<MarketPlacePlugin> { marketPlacePlugin });
        _pluginRepository.GetInstalledPluginVersion("testId").Returns(new Version("1.0.0"));

        var result = (
            await _getAvailablePluginsService.GetMarketPlugins(
                TestContext.Current.CancellationToken
            )
        ).Single();

        result.InstalledVersions.Single().Installed.ShouldBeFalse();
    }
}

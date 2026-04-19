using NSubstitute;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins.MarketPlace;
using ScriptBee.Marketplace.Client;
using ScriptBee.Service.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Tests.Plugins;

public class GetAvailablePluginsServiceTest
{
    private readonly IMarketPluginFetcher _marketPluginFetcher =
        Substitute.For<IMarketPluginFetcher>();

    private readonly GetAvailablePluginsService _getAvailablePluginsService;

    public GetAvailablePluginsServiceTest()
    {
        _getAvailablePluginsService = new GetAvailablePluginsService(_marketPluginFetcher);
    }

    [Fact]
    public async Task GetMarketPlugins_GetsProjectsFromFetcher()
    {
        var plugins = new List<MarketPlacePlugin>
        {
            new("id", "name", MarketPlacePluginType.Bundle, "description", [], []),
        };
        _marketPluginFetcher.GetProjectsAsync(Arg.Any<CancellationToken>()).Returns(plugins);

        var marketPlacePlugins = await _getAvailablePluginsService.GetMarketPlugins(
            TestContext.Current.CancellationToken
        );

        marketPlacePlugins.ShouldBe(plugins);
        await _marketPluginFetcher.Received(1).GetProjectsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetMarketPlugins_ReturnsEmptyList_WhenFetcherReturnsEmptyList()
    {
        _marketPluginFetcher.GetProjectsAsync(Arg.Any<CancellationToken>()).Returns([]);

        var result = await _getAvailablePluginsService.GetMarketPlugins(
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetMarketPlugins_ReturnsPlugin_WhenPluginExists()
    {
        var plugin = new MarketPlacePlugin(
            "id",
            "name",
            MarketPlacePluginType.Bundle,
            "description",
            [],
            []
        );
        _marketPluginFetcher
            .GetProjectsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<MarketPlacePlugin> { plugin });

        var result = await _getAvailablePluginsService.GetMarketPlugin(
            "id",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(plugin);
    }

    [Fact]
    public async Task GetMarketPlugins_ReturnsPluginNotFoundError_WhenPluginDoesNotExist()
    {
        var plugin = new MarketPlacePlugin(
            "id",
            "name",
            MarketPlacePluginType.Bundle,
            "description",
            [],
            []
        );
        _marketPluginFetcher
            .GetProjectsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<MarketPlacePlugin> { plugin });

        var result = await _getAvailablePluginsService.GetMarketPlugin(
            "id2",
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
        result.AsT1.ShouldBe(new PluginNotFoundError("id2"));
    }
}

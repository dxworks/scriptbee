using NSubstitute;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.MarketPlace;
using ScriptBee.Marketplace.Client.Errors;

namespace ScriptBee.Marketplace.Client.Tests;

public class PluginUrlFetcherTest
{
    private readonly IMarketPluginFetcher _marketPluginFetcher =
        Substitute.For<IMarketPluginFetcher>();

    private readonly PluginUrlFetcher _pluginUrlFetcher;

    public PluginUrlFetcherTest()
    {
        _pluginUrlFetcher = new PluginUrlFetcher(_marketPluginFetcher);
    }

    [Fact]
    public async Task GetPluginUrl_ReturnsPluginNotFoundError_WhenPluginNotFound()
    {
        var pluginId = new PluginId("nonExistentId", new Version());
        _marketPluginFetcher
            .GetProjectsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<MarketPlacePlugin>());

        var result = await _pluginUrlFetcher.GetPluginUrl(
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
        result.AsT1.ShouldBeEquivalentTo(new PluginNotFoundError(pluginId));
    }

    [Fact]
    public async Task GetPluginUrl_ReturnsPluginVersionNotFoundError_WhenVersionNotFound()
    {
        var pluginId = new PluginId("testId", new Version("2.0.0"));
        var project = new MarketPlacePlugin(
            "testId",
            "Test Plugin",
            MarketPlacePluginType.Plugin,
            "Desc",
            [],
            [new PluginVersion("url1", new Version("1.0.0"), "manifest1")]
        );
        _marketPluginFetcher
            .GetProjectsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<MarketPlacePlugin> { project });

        var result = await _pluginUrlFetcher.GetPluginUrl(
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBeTrue();
        result.AsT2.ShouldBeEquivalentTo(new PluginVersionNotFoundError(pluginId));
    }

    [Fact]
    public async Task GetPluginUrl_ReturnsCorrectUrl_WhenPluginAndVersionFound()
    {
        var pluginId = new PluginId("testId", new Version("2.0.0"));
        var project = new MarketPlacePlugin(
            "testId",
            "Test Plugin",
            MarketPlacePluginType.Plugin,
            "Desc",
            [],
            [
                new PluginVersion("url1", new Version("1.0.0"), "manifest1"),
                new PluginVersion("url2", new Version("2.0.0"), "manifest2"),
            ]
        );
        _marketPluginFetcher
            .GetProjectsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<MarketPlacePlugin> { project });

        var url = await _pluginUrlFetcher.GetPluginUrl(
            pluginId,
            TestContext.Current.CancellationToken
        );

        url.IsT0.ShouldBeTrue();
        url.ShouldBe("url2");
    }
}

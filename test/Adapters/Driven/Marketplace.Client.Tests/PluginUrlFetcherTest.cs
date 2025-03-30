using NSubstitute;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.MarketPlace;
using ScriptBee.Marketplace.Client.Exceptions;
using ScriptBee.Ports.Plugins;

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
    public void GetPluginUrl_ThrowsPluginNotFoundException_WhenPluginNotFound()
    {
        _marketPluginFetcher.GetProjectsAsync().Returns(new List<MarketPlacePlugin>());

        Action act = () => _pluginUrlFetcher.GetPluginUrl("nonExistentId", "1.0.0");

        act.ShouldThrow<PluginNotFoundException>().Message.ShouldBe("nonExistentId");
    }

    [Fact]
    public void GetPluginUrl_ThrowsPluginVersionNotFoundException_WhenVersionNotFound()
    {
        var project = new MarketPlacePlugin(
            "testId",
            "Test Plugin",
            MarketPlacePluginType.Plugin,
            "Desc",
            [],
            [new PluginVersion("url1", new Version("1.0.0"), "manifest1")]
        );
        _marketPluginFetcher.GetProjectsAsync().Returns(new List<MarketPlacePlugin> { project });

        Action act = () => _pluginUrlFetcher.GetPluginUrl("testId", "2.0.0");

        act.ShouldThrow<PluginVersionNotFoundException>().Message.ShouldBe("testId 2.0.0");
    }

    [Fact]
    public void GetPluginUrl_ReturnsCorrectUrl_WhenPluginAndVersionFound()
    {
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
        _marketPluginFetcher.GetProjectsAsync().Returns(new List<MarketPlacePlugin> { project });

        var url = _pluginUrlFetcher.GetPluginUrl("testId", "2.0.0");

        url.ShouldBe("url2");
    }

    [Fact]
    public void GetPluginUrl_ParsesVersionCorrectly()
    {
        var project = new MarketPlacePlugin(
            "testId",
            "Test Plugin",
            MarketPlacePluginType.Plugin,
            "Desc",
            [],
            [new PluginVersion("url1", new Version("1.0.0"), "manifest1")]
        );
        _marketPluginFetcher.GetProjectsAsync().Returns(new List<MarketPlacePlugin> { project });

        var url = _pluginUrlFetcher.GetPluginUrl("testId", "1.0.0");

        url.ShouldBe("url1");
    }

    [Fact]
    public void GetPluginUrl_FindsPluginWithCorrectId()
    {
        var project1 = new MarketPlacePlugin(
            "id1",
            "Plugin 1",
            MarketPlacePluginType.Plugin,
            "Desc 1",
            [],
            [new PluginVersion("url1", new Version("1.0.0"), "manifest1")]
        );
        var project2 = new MarketPlacePlugin(
            "id2",
            "Plugin 2",
            MarketPlacePluginType.Plugin,
            "Desc 2",
            [],
            [new PluginVersion("url2", new Version("2.0.0"), "manifest2")]
        );
        _marketPluginFetcher
            .GetProjectsAsync()
            .Returns(new List<MarketPlacePlugin> { project1, project2 });

        var url = _pluginUrlFetcher.GetPluginUrl("id2", "2.0.0");

        url.ShouldBe("url2");
    }
}

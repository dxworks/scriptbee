using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Moq;
using ScriptBee.Marketplace.Client.Data;
using ScriptBee.Marketplace.Client.Services;
using ScriptBeeWebApp.Data.Exceptions;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Services;

public class PluginUrlFetcherTests
{
    private readonly Mock<IMarketPluginFetcher> _marketPluginFetcher;
    private readonly PluginUrlFetcher _pluginUrlFetcher;
    private readonly Fixture _fixture;

    public PluginUrlFetcherTests()
    {
        _marketPluginFetcher = new Mock<IMarketPluginFetcher>();
        _fixture = new Fixture();

        _pluginUrlFetcher = new PluginUrlFetcher(_marketPluginFetcher.Object);
    }

    [Fact]
    public void GivenEmptyProjects_WhenGetPluginUrlAsync_ThenPluginNotFoundExceptionIsThrown()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync())
            .Returns(Enumerable.Empty<MarketPlaceProject>());

        Assert.Throws<PluginNotFoundException>(() => _pluginUrlFetcher.GetPluginUrl("plugin", "1.0.0"));
    }

    [Fact]
    public void GivenProjectWithNoPluginId_WhenGetPluginUrlAsync_ThenPluginNotFoundExceptionIsThrown()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync())
            .Returns(new List<MarketPlaceProject>
            {
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Create<MarketPlaceProject>(),
            });

        Assert.Throws<PluginNotFoundException>(() =>
            _pluginUrlFetcher.GetPluginUrl("plugin", "1.0.0"));
    }

    [Fact]
    public void GivenProjectWithWrongVersion_WhenGetPluginUrlAsync_ThenPluginNotFoundExceptionIsThrown()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync())
            .Returns(new List<MarketPlaceProject>
            {
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Build<MarketPlaceProject>()
                    .With(p => p.Id, "plugin")
                    .Create(),
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Create<MarketPlaceProject>(),
            });

        Assert.Throws<PluginVersionNotFoundException>(() => _pluginUrlFetcher.GetPluginUrl("plugin", "1.0.0"));
    }

    [Fact]
    public void GivenProjectWithCorrectVersion_WhenGetPluginUrlAsync_ThenPluginUrlIsReturned()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync())
            .Returns(new List<MarketPlaceProject>
            {
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Build<MarketPlaceProject>()
                    .With(p => p.Id, "plugin")
                    .With(p => p.Versions, new List<PluginVersion>
                    {
                        _fixture.Build<PluginVersion>()
                            .With(v => v.Version, new Version(1, 0, 0))
                            .With(v => v.Url, "url")
                            .Create()
                    })
                    .Create(),
            });

        var url = _pluginUrlFetcher.GetPluginUrl("plugin", "1.0.0");

        Assert.Equal("url", url);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public async Task GivenEmptyProjects_WhenGetPluginUrlAsync_ThenPluginNotFoundExceptionIsThrown()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<MarketPlaceProject>());

        await Assert.ThrowsAsync<PluginNotFoundException>(() =>
            _pluginUrlFetcher.GetPluginUrlAsync("plugin", "1.0.0", It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task GivenProjectWithNoPluginId_WhenGetPluginUrlAsync_ThenPluginNotFoundExceptionIsThrown()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MarketPlaceProject>
            {
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Create<MarketPlaceProject>(),
            });

        await Assert.ThrowsAsync<PluginNotFoundException>(() =>
            _pluginUrlFetcher.GetPluginUrlAsync("plugin", "1.0.0", It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task GivenProjectWithWrongVersion_WhenGetPluginUrlAsync_ThenPluginNotFoundExceptionIsThrown()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MarketPlaceProject>
            {
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Build<MarketPlaceProject>()
                    .With(p => p.Id, "plugin")
                    .Create(),
                _fixture.Create<MarketPlaceProject>(),
                _fixture.Create<MarketPlaceProject>(),
            });

        await Assert.ThrowsAsync<PluginVersionNotFoundException>(() =>
            _pluginUrlFetcher.GetPluginUrlAsync("plugin", "1.0.0", It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task GivenProjectWithCorrectVersion_WhenGetPluginUrlAsync_ThenPluginUrlIsReturned()
    {
        _marketPluginFetcher.Setup(x => x.GetProjectsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MarketPlaceProject>
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

        var url = await _pluginUrlFetcher.GetPluginUrlAsync("plugin", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Equal("url", url);
    }
}

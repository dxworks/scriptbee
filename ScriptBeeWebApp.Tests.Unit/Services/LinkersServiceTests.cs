using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using DxWorks.ScriptBee.Plugin.Api;
using Moq;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Services;

public class LinkersServiceTests
{
    private readonly Fixture _fixture;

    private readonly LinkersService _linkersService;
    private readonly Mock<IPluginRepository> _pluginRepositoryMock;

    public LinkersServiceTests()
    {
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _fixture = new Fixture();

        _linkersService = new LinkersService(_pluginRepositoryMock.Object);
    }

    [Fact]
    public void GivenLinkers_WhenGetSupportedLinkers_ThenLinkersNamesAreReturned()
    {
        var manifests = new List<PluginManifest>
        {
            CreateLinker("linker1"),
            CreateLinker("linker2")
        };

        _pluginRepositoryMock.Setup(x => x.GetLoadedPluginManifests())
            .Returns(manifests);

        var supportedLinkers = _linkersService.GetSupportedLinkers().ToList();

        Assert.Equal(2, supportedLinkers.Count);
        Assert.Equal("linker1", supportedLinkers[0]);
        Assert.Equal("linker2", supportedLinkers[1]);
    }

    [Fact]
    public void GivenInvalidLinkerName_WhenGetLinker_ThenNullIsReturned()
    {
        _pluginRepositoryMock.Setup(x => x.GetPlugin(It.IsAny<Func<IModelLinker, bool>>(), null))
            .Returns((IModelLinker?)null);

        var linker = _linkersService.GetLinker("invalidLinker");

        Assert.Null(linker);
    }

    [Fact]
    public void GivenValidLinkerName_WhenGetLinker_ThenLinkerIsReturned()
    {
        var expectedLinker = new Mock<IModelLinker>().Object;

        _pluginRepositoryMock.Setup(x => x.GetPlugin(It.IsAny<Func<IModelLinker, bool>>(), null))
            .Returns(expectedLinker);

        var linker = _linkersService.GetLinker("linker");

        Assert.Equal(expectedLinker, linker);
    }

    private PluginManifest CreateLinker(string name)
    {
        var linkerPluginExtensionPoint = _fixture.Build<LinkerPluginExtensionPoint>()
            .With(e => e.Kind, PluginKind.Linker)
            .Create();

        return _fixture.Build<PluginManifest>()
            .With(m => m.Name, name)
            .With(m => m.ExtensionPoints, new List<PluginExtensionPoint> { linkerPluginExtensionPoint })
            .Create();
    }
}

using System;
using System.Collections.Generic;
using Moq;
using ScriptBee.Plugin;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginManagerTests
{
    private readonly Mock<IPluginManifestReader> _pluginManifestReaderMock;
    private readonly Mock<IPluginLoaderFactory> _pluginLoaderFactoryMock;
    private readonly Mock<IPluginRepository> _pluginRepositoryMock;

    private readonly PluginManager _pluginManager;

    public PluginManagerTests()
    {
        var loggerMock = new Mock<ILogger>();

        _pluginManifestReaderMock = new Mock<IPluginManifestReader>();
        _pluginLoaderFactoryMock = new Mock<IPluginLoaderFactory>();
        _pluginRepositoryMock = new Mock<IPluginRepository>();

        _pluginManager = new PluginManager(loggerMock.Object, _pluginManifestReaderMock.Object,
            _pluginLoaderFactoryMock.Object, _pluginRepositoryMock.Object);
    }

    [Fact]
    public void GivenEmptyManifest_WhenLoadPlugins_ThenNoPluginIsLoaded()
    {
        _pluginManifestReaderMock.Setup(x => x.ReadManifests(It.IsAny<string>())).Returns(new List<PluginManifest>());

        _pluginManager.LoadPlugins("path");

        _pluginLoaderFactoryMock.Verify(x => x.GetPluginLoader(It.IsAny<Type>()), Times.Never);
    }
}

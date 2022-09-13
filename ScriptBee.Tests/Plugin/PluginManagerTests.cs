using System;
using System.Collections.Generic;
using Moq;
using ScriptBee.Plugin;
using ScriptBee.Tests.Plugin.Internals;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginManagerTests
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<IPluginReader> _pluginReaderMock;
    private readonly Mock<IPluginLoader> _pluginLoaderMock;

    private readonly PluginManager _pluginManager;

    public PluginManagerTests()
    {
        _loggerMock = new Mock<ILogger>();
        _pluginReaderMock = new Mock<IPluginReader>();
        _pluginLoaderMock = new Mock<IPluginLoader>();

        _pluginManager = new PluginManager(_loggerMock.Object, _pluginReaderMock.Object, _pluginLoaderMock.Object);
    }

    [Fact]
    public void GivenEmptyPlugins_WhenLoadPlugins_ThenNoPluginsLoaded()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins("path"))
            .Returns(new List<Models.Plugin>());

        _pluginManager.LoadPlugins("path");

        _pluginLoaderMock.Verify(x => x.Load(It.IsAny<Models.Plugin>()), Times.Never());
    }

    [Fact]
    public void GivenAllValidPlugins_WhenLoadPlugins_ThenAllPluginsAreLoaded()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins("path"))
            .Returns(new List<Models.Plugin>
            {
                new TestPlugin(),
                new TestPlugin(),
                new TestPlugin(),
            });

        _pluginManager.LoadPlugins("path");

        _pluginLoaderMock.Verify(x => x.Load(It.IsAny<Models.Plugin>()), Times.Exactly(3));
    }

    [Fact]
    public void GivenSomeInvalidPlugins_WhenLoadPlugins_ThenAllValidPluginsAreLoaded()
    {
        var expectedException = new Exception("Test exception");

        Models.Plugin testPlugin1 = new TestPlugin();
        Models.Plugin testPlugin2 = new TestPlugin();
        Models.Plugin testPlugin3 = new TestPlugin();

        _pluginReaderMock.Setup(x => x.ReadPlugins("path"))
            .Returns(new List<Models.Plugin>
            {
                testPlugin1,
                testPlugin2,
                testPlugin3,
            });
        _pluginLoaderMock.Setup(l => l.Load(testPlugin2)).Throws(expectedException);

        _pluginManager.LoadPlugins("path");

        _loggerMock.Verify(l => l.Error(expectedException, "Failed to load plugin {plugin}", testPlugin2),
            Times.Once());
    }
}

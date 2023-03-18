using System;
using System.Collections.Generic;
using Moq;
using ScriptBee.Config;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Installer;
using ScriptBee.ProjectContext;
using ScriptBee.Tests.Plugin.Internals;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginManagerTests
{
    private readonly Mock<IPluginReader> _pluginReaderMock;
    private readonly Mock<IPluginLoader> _pluginLoaderMock;
    private readonly Mock<IPluginUninstaller> _pluginUninstallerMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<IProjectFileStructureManager> _projectFileStructureManagerMock;

    private readonly PluginManager _pluginManager;

    public PluginManagerTests()
    {
        _pluginReaderMock = new Mock<IPluginReader>();
        _pluginLoaderMock = new Mock<IPluginLoader>();
        _pluginUninstallerMock = new Mock<IPluginUninstaller>();
        _projectFileStructureManagerMock = new Mock<IProjectFileStructureManager>();
        _loggerMock = new Mock<ILogger>();

        _pluginManager = new PluginManager(_pluginReaderMock.Object, _pluginLoaderMock.Object,
            _pluginUninstallerMock.Object, _projectFileStructureManagerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GivenEmptyPlugins_WhenLoadPlugins_ThenNoPluginsLoaded()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>());

        _pluginManager.LoadPlugins();

        _pluginLoaderMock.Verify(x => x.Load(It.IsAny<Models.Plugin>()), Times.Never());
    }

    [Fact]
    public void GivenAllValidPlugins_WhenLoadPlugins_ThenAllPluginsAreLoaded()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>
            {
                new TestPlugin("id", new Version(0, 0, 0, 1)),
                new TestPlugin("id", new Version(0, 0, 0, 2)),
                new TestPlugin("id", new Version(0, 0, 0, 3)),
            });

        _pluginManager.LoadPlugins();

        _pluginLoaderMock.Verify(x => x.Load(It.IsAny<Models.Plugin>()), Times.Exactly(3));
        _projectFileStructureManagerMock.Verify(x => x.CreateScriptBeeFolderStructure(), Times.Once());
    }

    [Fact]
    public void GivenSomeInvalidPlugins_WhenLoadPlugins_ThenAllValidPluginsAreLoaded()
    {
        var expectedException = new Exception("Test exception");

        Models.Plugin testPlugin1 = new TestPlugin("id", new Version(0, 0, 0, 1));
        Models.Plugin testPlugin2 = new TestPlugin("id", new Version(0, 0, 1, 1));
        Models.Plugin testPlugin3 = new TestPlugin("id", new Version(0, 0, 2, 1));

        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>
            {
                testPlugin1,
                testPlugin2,
                testPlugin3,
            });
        _pluginLoaderMock.Setup(l => l.Load(testPlugin2)).Throws(expectedException);

        _pluginManager.LoadPlugins();

        _loggerMock.Verify(l => l.Error(expectedException, "Failed to load plugin {Plugin}", testPlugin2),
            Times.Once());
        _projectFileStructureManagerMock.Verify(x => x.CreateScriptBeeFolderStructure(), Times.Once());
    }

    [Fact]
    public void WhenLoadPlugins_ThenMarkedForDeletePluginsAreUninstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>());

        _pluginManager.LoadPlugins();

        _pluginUninstallerMock.Verify(x => x.DeleteMarkedPlugins(), Times.Once());
        _projectFileStructureManagerMock.Verify(x => x.CreateScriptBeeFolderStructure(), Times.Once());
    }
}

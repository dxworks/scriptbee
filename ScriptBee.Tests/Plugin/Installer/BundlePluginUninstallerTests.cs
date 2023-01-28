using Moq;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Installer;
using ScriptBee.Plugin.Manifest;
using Serilog;
using Xunit;
using static ScriptBee.Tests.Utils.PluginUtils;

namespace ScriptBee.Tests.Plugin.Installer;

public class BundlePluginUninstallerTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IPluginReader> _pluginReaderMock;
    private readonly Mock<IPluginUninstaller> _pluginUninstaller;
    private readonly BundlePluginUninstaller _bundlePluginUninstaller;

    public BundlePluginUninstallerTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _pluginReaderMock = new Mock<IPluginReader>();
        _pluginUninstaller = new Mock<IPluginUninstaller>();
        var loggerMock = new Mock<ILogger>();

        _bundlePluginUninstaller =
            new BundlePluginUninstaller(_fileServiceMock.Object, _pluginReaderMock.Object, _pluginUninstaller.Object,
                loggerMock.Object);
    }

    [Fact]
    public void GivenSimplePlugin_WhenUninstall_ThenPluginIsUninstalled()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "plugin@1.0.0"))
            .Returns("plugin_path");

        var versions = _bundlePluginUninstaller.Uninstall("plugin", "1.0.0");

        Assert.Equal(1, versions.Count);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("plugin", versions[0].PluginId);
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path"), Times.Once());
    }

    [Fact]
    public void GivenPluginWithNoManifest_WhenUninstall_ThenPluginFolderIsDeleted()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "plugin@1.0.0"))
            .Returns("plugin_path");
        _pluginReaderMock.Setup(x => x.ReadPlugin("plugin_path")).Returns<Models.Plugin?>(null);

        var versions = _bundlePluginUninstaller.Uninstall("plugin", "1.0.0");


        Assert.Equal(1, versions.Count);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("plugin", versions[0].PluginId);
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path"), Times.Once());
    }

    [Fact]
    public void GivenBundleWithOnePlugin_WhenUninstall_ThenPluginIsUninstalled()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "bundle@1.0.0"))
            .Returns("bundle_path");
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginId@1.0.0"))
            .Returns("plugin_path");
        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_path"))
            .Returns(CreateBundlePlugin("bundle", "1.0.0",
                new TestBundlePlugin(PluginKind.Plugin, "pluginId", "1.0.0")));

        var versions = _bundlePluginUninstaller.Uninstall("bundle", "1.0.0");

        Assert.Equal(2, versions.Count);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("bundle", versions[0].PluginId);
        Assert.Equal("1.0.0", versions[1].Version);
        Assert.Equal("pluginId", versions[1].PluginId);
        _pluginUninstaller.Verify(x => x.Uninstall("bundle_path"), Times.Once());
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path"), Times.Once());
    }

    [Fact]
    public void GivenBundleWithMultiplePlugins_WhenUninstall_ThenPluginsAreUninstalled()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "bundle@1.0.0"))
            .Returns("bundle_path");
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginId1@1.0.0"))
            .Returns("plugin_path1");
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginId2@1.0.0"))
            .Returns("plugin_path2");
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginId3@1.0.0"))
            .Returns("plugin_path3");
        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_path"))
            .Returns(CreateBundlePlugin("bundle", "1.0.0",
                new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0"),
                new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0"),
                new TestBundlePlugin(PluginKind.Plugin, "pluginId3", "1.0.0")));

        var versions = _bundlePluginUninstaller.Uninstall("bundle", "1.0.0");
        
        Assert.Equal(4, versions.Count);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("bundle", versions[0].PluginId);
        Assert.Equal("1.0.0", versions[1].Version);
        Assert.Equal("pluginId1", versions[1].PluginId);
        Assert.Equal("1.0.0", versions[2].Version);
        Assert.Equal("pluginId2", versions[2].PluginId);
        Assert.Equal("1.0.0", versions[3].Version);
        Assert.Equal("pluginId3", versions[3].PluginId);
        _pluginUninstaller.Verify(x => x.Uninstall("bundle_path"), Times.Once());
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path1"), Times.Once());
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path2"), Times.Once());
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path3"), Times.Once());
    }

    [Fact]
    public void GivenBundleOfBundles_WhenUninstall_ThenPluginsAreUninstalled()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "bundle@1.0.0"))
            .Returns("bundle_path");
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginId1@1.0.0"))
            .Returns("plugin_path1");
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginId2@1.0.0"))
            .Returns("plugin_path2");
        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_path"))
            .Returns(CreateBundlePlugin("bundle", "1.0.0",
                new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0")));
        _pluginReaderMock.Setup(x => x.ReadPlugin("plugin_path1"))
            .Returns(CreateBundlePlugin("pluginId1", "1.0.0",
                new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0")));

        var versions = _bundlePluginUninstaller.Uninstall("bundle", "1.0.0");

        Assert.Equal(3, versions.Count);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("bundle", versions[0].PluginId);
        Assert.Equal("1.0.0", versions[1].Version);
        Assert.Equal("pluginId1", versions[1].PluginId);
        Assert.Equal("1.0.0", versions[2].Version);
        Assert.Equal("pluginId2", versions[2].PluginId);
        _pluginUninstaller.Verify(x => x.Uninstall("bundle_path"), Times.Once());
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path1"), Times.Once());
        _pluginUninstaller.Verify(x => x.Uninstall("plugin_path2"), Times.Once());
    }
}

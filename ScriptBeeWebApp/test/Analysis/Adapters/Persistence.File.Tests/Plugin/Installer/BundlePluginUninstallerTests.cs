using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Persistence.File.Plugin.Installer;
using ScriptBee.Ports.Plugins;
using static ScriptBee.Tests.Common.Plugin.PluginUtils;

namespace ScriptBee.Persistence.File.Tests.Plugin.Installer;

public class BundlePluginUninstallerTests
{
    private readonly IFileService _fileService = Substitute.For<IFileService>();
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();
    private readonly IPluginUninstaller _pluginUninstaller = Substitute.For<IPluginUninstaller>();

    private readonly ILogger<BundlePluginUninstaller> _logger = Substitute.For<
        ILogger<BundlePluginUninstaller>
    >();

    private readonly BundlePluginUninstaller _bundlePluginUninstaller;

    public BundlePluginUninstallerTests()
    {
        _bundlePluginUninstaller = new BundlePluginUninstaller(
            _fileService,
            _pluginReader,
            _pluginUninstaller,
            _logger
        );
    }

    [Fact]
    public void GivenSimplePlugin_WhenUninstall_ThenPluginIsUninstalled()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "plugin@1.0.0")
            .Returns("plugin_path");

        var versions = _bundlePluginUninstaller.Uninstall("plugin", "1.0.0");

        Assert.Single(versions);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("plugin", versions[0].PluginId);
        _pluginUninstaller.Received(1).Uninstall("plugin_path");
    }

    [Fact]
    public void GivenPluginWithNoManifest_WhenUninstall_ThenPluginFolderIsDeleted()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "plugin@1.0.0")
            .Returns("plugin_path");
        _pluginReader.ReadPlugin("plugin_path").Returns((Domain.Model.Plugin.Plugin?)null);

        var versions = _bundlePluginUninstaller.Uninstall("plugin", "1.0.0");

        Assert.Single(versions);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("plugin", versions[0].PluginId);
        _pluginUninstaller.Received(1).Uninstall("plugin_path");
    }

    [Fact]
    public void GivenBundleWithOnePlugin_WhenUninstall_ThenPluginIsUninstalled()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "bundle@1.0.0")
            .Returns("bundle_path");
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginId@1.0.0")
            .Returns("plugin_path");
        _pluginReader
            .ReadPlugin("bundle_path")
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId", "1.0.0")
                )
            );

        var versions = _bundlePluginUninstaller.Uninstall("bundle", "1.0.0");

        Assert.Equal(2, versions.Count);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("bundle", versions[0].PluginId);
        Assert.Equal("1.0.0", versions[1].Version);
        Assert.Equal("pluginId", versions[1].PluginId);
        _pluginUninstaller.Received(1).Uninstall("bundle_path");
        _pluginUninstaller.Received(1).Uninstall("plugin_path");
    }

    [Fact]
    public void GivenBundleWithMultiplePlugins_WhenUninstall_ThenPluginsAreUninstalled()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "bundle@1.0.0")
            .Returns("bundle_path");
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginId1@1.0.0")
            .Returns("plugin_path1");
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginId2@1.0.0")
            .Returns("plugin_path2");
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginId3@1.0.0")
            .Returns("plugin_path3");
        _pluginReader
            .ReadPlugin("bundle_path")
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0"),
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0"),
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId3", "1.0.0")
                )
            );

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
        _pluginUninstaller.Received(1).Uninstall("bundle_path");
        _pluginUninstaller.Received(1).Uninstall("plugin_path1");
        _pluginUninstaller.Received(1).Uninstall("plugin_path2");
        _pluginUninstaller.Received(1).Uninstall("plugin_path3");
    }

    [Fact]
    public void GivenBundleOfBundles_WhenUninstall_ThenPluginsAreUninstalled()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "bundle@1.0.0")
            .Returns("bundle_path");
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginId1@1.0.0")
            .Returns("plugin_path1");
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginId2@1.0.0")
            .Returns("plugin_path2");
        _pluginReader
            .ReadPlugin("bundle_path")
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0")
                )
            );
        _pluginReader
            .ReadPlugin("plugin_path1")
            .Returns(
                CreateBundlePlugin(
                    "pluginId1",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0")
                )
            );

        var versions = _bundlePluginUninstaller.Uninstall("bundle", "1.0.0");

        Assert.Equal(3, versions.Count);
        Assert.Equal("1.0.0", versions[0].Version);
        Assert.Equal("bundle", versions[0].PluginId);
        Assert.Equal("1.0.0", versions[1].Version);
        Assert.Equal("pluginId1", versions[1].PluginId);
        Assert.Equal("1.0.0", versions[2].Version);
        Assert.Equal("pluginId2", versions[2].PluginId);
        _pluginUninstaller.Received(1).Uninstall("bundle_path");
        _pluginUninstaller.Received(1).Uninstall("plugin_path1");
        _pluginUninstaller.Received(1).Uninstall("plugin_path2");
    }
}

using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Tests.Common;
using static ScriptBee.Tests.Common.Plugins.PluginUtils;

namespace ScriptBee.Plugins.Installer.Tests;

public class BundlePluginUninstallerTests : IClassFixture<TempDirFixture>
{
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();
    private readonly IPluginUninstaller _pluginUninstaller = Substitute.For<IPluginUninstaller>();

    private readonly ILogger<BundlePluginUninstaller> _logger = Substitute.For<
        ILogger<BundlePluginUninstaller>
    >();

    private readonly BundlePluginUninstaller _bundlePluginUninstaller;
    private readonly TempDirFixture _fixture;

    public BundlePluginUninstallerTests(TempDirFixture fixture)
    {
        _fixture = fixture;
        _bundlePluginUninstaller = new BundlePluginUninstaller(
            _pluginReader,
            _pluginUninstaller,
            _logger
        );
    }

    [Fact]
    public void GivenSimplePlugin_WhenUninstall_ThenPluginIsUninstalled()
    {
        var pluginsPath = _fixture.CreateSubFolder("simple_uninstall");
        var expectedPath = Path.Combine(pluginsPath, "plugin@1.0.0");

        var versions = _bundlePluginUninstaller.Uninstall(
            new PluginId("plugin", new Version("1.0.0")),
            pluginsPath
        );

        versions.Count.ShouldBe(1);
        versions[0].Version.ShouldBe(new Version("1.0.0"));
        versions[0].Name.ShouldBe("plugin");
        _pluginUninstaller.Received(1).Uninstall(expectedPath);
    }

    [Fact]
    public void GivenPluginWithNoManifest_WhenUninstall_ThenPluginFolderIsDeleted()
    {
        var pluginsPath = _fixture.CreateSubFolder("no_manifest_uninstall");
        var expectedPath = Path.Combine(pluginsPath, "plugin@1.0.0");
        _pluginReader.ReadPlugin(expectedPath).Returns((Plugin?)null);

        var versions = _bundlePluginUninstaller.Uninstall(
            new PluginId("plugin", new Version("1.0.0")),
            pluginsPath
        );

        versions.Count.ShouldBe(1);
        versions[0].Version.ShouldBe(new Version("1.0.0"));
        versions[0].Name.ShouldBe("plugin");
        _pluginUninstaller.Received(1).Uninstall(expectedPath);
    }

    [Fact]
    public void GivenBundleWithOnePlugin_WhenUninstall_ThenPluginIsUninstalled()
    {
        var pluginsPath = _fixture.CreateSubFolder("bundle_one_uninstall");
        var bundlePath = Path.Combine(pluginsPath, "bundle@1.0.0");
        var pluginPath = Path.Combine(pluginsPath, "pluginId@1.0.0");

        _pluginReader
            .ReadPlugin(bundlePath)
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId", "1.0.0")
                )
            );

        var versions = _bundlePluginUninstaller.Uninstall(
            new PluginId("bundle", new Version("1.0.0")),
            pluginsPath
        );

        versions.Count.ShouldBe(2);
        versions[0].Version.ShouldBe(new Version("1.0.0"));
        versions[0].Name.ShouldBe("bundle");
        versions[1].Version.ShouldBe(new Version("1.0.0"));
        versions[1].Name.ShouldBe("pluginId");
        _pluginUninstaller.Received(1).Uninstall(bundlePath);
        _pluginUninstaller.Received(1).Uninstall(pluginPath);
    }

    [Fact]
    public void GivenBundleWithMultiplePlugins_WhenUninstall_ThenPluginsAreUninstalled()
    {
        var pluginsPath = _fixture.CreateSubFolder("bundle_multiple_uninstall");
        var bundlePath = Path.Combine(pluginsPath, "bundle@1.0.0");
        var pluginPath1 = Path.Combine(pluginsPath, "pluginId1@1.0.0");
        var pluginPath2 = Path.Combine(pluginsPath, "pluginId2@1.0.0");
        var pluginPath3 = Path.Combine(pluginsPath, "pluginId3@1.0.0");

        _pluginReader
            .ReadPlugin(bundlePath)
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0"),
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0"),
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId3", "1.0.0")
                )
            );

        var versions = _bundlePluginUninstaller.Uninstall(
            new PluginId("bundle", new Version("1.0.0")),
            pluginsPath
        );

        versions.Count.ShouldBe(4);
        versions[0].Version.ShouldBe(new Version("1.0.0"));
        versions[0].Name.ShouldBe("bundle");
        versions[1].Version.ShouldBe(new Version("1.0.0"));
        versions[1].Name.ShouldBe("pluginId1");
        versions[2].Version.ShouldBe(new Version("1.0.0"));
        versions[2].Name.ShouldBe("pluginId2");
        versions[3].Version.ShouldBe(new Version("1.0.0"));
        versions[3].Name.ShouldBe("pluginId3");
        _pluginUninstaller.Received(1).Uninstall(bundlePath);
        _pluginUninstaller.Received(1).Uninstall(pluginPath1);
        _pluginUninstaller.Received(1).Uninstall(pluginPath2);
        _pluginUninstaller.Received(1).Uninstall(pluginPath3);
    }

    [Fact]
    public void GivenBundleOfBundles_WhenUninstall_ThenPluginsAreUninstalled()
    {
        var pluginsPath = _fixture.CreateSubFolder("bundle_of_bundles_uninstall");
        var bundlePath = Path.Combine(pluginsPath, "bundle@1.0.0");
        var pluginPath1 = Path.Combine(pluginsPath, "pluginId1@1.0.0");
        var pluginPath2 = Path.Combine(pluginsPath, "pluginId2@1.0.0");

        _pluginReader
            .ReadPlugin(bundlePath)
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0")
                )
            );
        _pluginReader
            .ReadPlugin(pluginPath1)
            .Returns(
                CreateBundlePlugin(
                    "pluginId1",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0")
                )
            );

        var versions = _bundlePluginUninstaller.Uninstall(
            new PluginId("bundle", new Version("1.0.0")),
            pluginsPath
        );

        versions.Count.ShouldBe(3);
        versions[0].Version.ShouldBe(new Version("1.0.0"));
        versions[0].Name.ShouldBe("bundle");
        versions[1].Version.ShouldBe(new Version("1.0.0"));
        versions[1].Name.ShouldBe("pluginId1");
        versions[2].Version.ShouldBe(new Version("1.0.0"));
        versions[2].Name.ShouldBe("pluginId2");
        _pluginUninstaller.Received(1).Uninstall(bundlePath);
        _pluginUninstaller.Received(1).Uninstall(pluginPath1);
        _pluginUninstaller.Received(1).Uninstall(pluginPath2);
    }
}

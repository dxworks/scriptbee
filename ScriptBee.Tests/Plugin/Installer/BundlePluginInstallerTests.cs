using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ScriptBee.Config;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Installer;
using ScriptBee.Plugin.Manifest;
using ScriptBee.Tests.Plugin.Internals;
using Serilog;
using Xunit;
using static ScriptBee.Tests.Utils.PluginUtils;
using Range = Moq.Range;

namespace ScriptBee.Tests.Plugin.Installer;

public class BundlePluginInstallerTests
{
    private readonly Mock<IPluginReader> _pluginReaderMock;
    private readonly Mock<ISimplePluginInstaller> _simplePluginInstallerMock;
    private readonly Mock<IPluginUninstaller> _pluginUninstallerMock;
    private readonly Mock<IPluginUrlFetcher> _pluginFetcherMock;
    private readonly BundlePluginInstaller _bundlePluginInstaller;

    public BundlePluginInstallerTests()
    {
        _pluginReaderMock = new Mock<IPluginReader>();
        _simplePluginInstallerMock = new Mock<ISimplePluginInstaller>();
        _pluginUninstallerMock = new Mock<IPluginUninstaller>();
        _pluginFetcherMock = new Mock<IPluginUrlFetcher>();
        var loggerMock = new Mock<ILogger>();

        _bundlePluginInstaller =
            new BundlePluginInstaller(_pluginReaderMock.Object, _simplePluginInstallerMock.Object,
                _pluginUninstallerMock.Object, _pluginFetcherMock.Object, loggerMock.Object);
    }

    #region Simple Plugin Tests

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePlugin_WhenInstall_ThenPluginIsInstalledSuccessfully(PluginList pluginList)
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(pluginList.Plugins);
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginId", "1.0.0"))
            .Returns("url");
        _simplePluginInstallerMock.Setup(x => x.Install("url", "pluginId", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("plugin_folder");

        var pluginFolders =
            await _bundlePluginInstaller.Install("pluginId", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Equal("plugin_folder", pluginFolders.Single());
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndUrlFetchFails_WhenInstall_ThenPluginIsNotInstalled(PluginList pluginList)
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(pluginList.Plugins);
        _pluginFetcherMock.Setup(x =>
                x.GetPluginUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception());

        await Assert.ThrowsAsync<Exception>(() =>
            _bundlePluginInstaller.Install("pluginId", "1.0.0", It.IsAny<CancellationToken>()));

        _pluginUninstallerMock.Verify(x => x.Uninstall(It.IsAny<string>()), Times.Never());
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndInstallFails_WhenInstall_ThenPluginIsNotInstalled(PluginList pluginList)
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(pluginList.Plugins);
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginId", "1.0.0"))
            .Returns("url");
        _simplePluginInstallerMock.Setup(x =>
                x.Install(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        await Assert.ThrowsAsync<Exception>(() =>
            _bundlePluginInstaller.Install("pluginId", "1.0.0", It.IsAny<CancellationToken>()));

        _pluginUninstallerMock.Verify(x => x.Uninstall(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task GivenSimplePluginAlreadyInstalled_WhenInstall_ThenNoPluginIsUninstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>
            {
                CreatePlugin("pluginName", "1.0.0"),
            });
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginName", "1.0.0"))
            .Returns("url");

        var pluginFolders = await _bundlePluginInstaller.Install("pluginName", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Empty(pluginFolders);
        _simplePluginInstallerMock.Verify(
            x => x.Install(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never());
        _pluginUninstallerMock.Verify(x => x.Uninstall(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task GivenSimplePluginVersionAndExistingNewVersion_WhenInstall_ThenPluginIsNotInstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>
            {
                CreatePlugin("pluginName", "10.0.0"),
            });
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginName", "1.0.0"))
            .Returns("url");

        var pluginFolders = await _bundlePluginInstaller.Install("pluginName", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Empty(pluginFolders);
        _simplePluginInstallerMock.Verify(
            x => x.Install(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Fact]
    public async Task GivenSimplePluginAndExistingOldVersion_WhenInstall_ThenOldVersionsAreUninstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(
            new List<Models.Plugin>
            {
                CreatePlugin("pluginName", "1.0.0", "plugin_folder_1"),
                CreatePlugin("pluginName", "14.0.0", "plugin_folder_2"),
                CreatePlugin("pluginName", "14.3.0", "plugin_folder_3"),
                CreatePlugin("pluginName", "1.2.3", "plugin_folder_4"),
                CreatePlugin("pluginName", "1.5.0", "plugin_folder_5"),
            });
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginName", "14.3.1"))
            .Returns("url");
        _simplePluginInstallerMock.Setup(x => x.Install("url", "pluginName", "14.3.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync("plugin_folder");

        var pluginFolders = await _bundlePluginInstaller.Install("pluginName", "14.3.1", It.IsAny<CancellationToken>());

        Assert.Equal("plugin_folder", pluginFolders.Single());
        for (var i = 1; i <= 5; i++)
        {
            var i1 = i;
            _pluginUninstallerMock.Verify(x => x.Uninstall($"plugin_folder_{i1}"), Times.Once());
        }
    }

    private static Models.Plugin CreatePlugin(string id, string version, string path = "path")
    {
        return new TestPlugin(id, new Version(version), path);
    }

    public sealed record PluginList(List<Models.Plugin> Plugins);

    private class BundlePluginInstallerSimpleTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new PluginList(new List<Models.Plugin>()) };
            yield return new object[]
                { new PluginList(new List<Models.Plugin> { new TestPlugin("pluginName", new Version(1, 0, 0)) }) };
            yield return new object[]
            {
                new PluginList(new List<Models.Plugin>
                {
                    new TestPlugin("pluginName", new Version(1, 0, 0)),
                    new TestPlugin("pluginName", new Version(1, 0, 1))
                })
            };
            yield return new object[]
            {
                new PluginList(new List<Models.Plugin>
                {
                    new TestPlugin("pluginName2", new Version(1, 0, 0)),
                    new TestPlugin("pluginName3", new Version(1, 0, 0)),
                    new TestPlugin("pluginName3", new Version(1, 0, 5))
                })
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion

    #region Bundle Plugin Tests

    [Fact]
    public async Task GivenBundlePluginThatHasNoPluginInFolder_WhenInstall_ThenOnlyFolderIsInstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(new List<Models.Plugin>());
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("bundle", "1.0.0"))
            .Returns("url");
        _simplePluginInstallerMock
            .Setup(x => x.Install("url", "bundle", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("bundle_folder");
        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_folder"))
            .Returns<Models.Plugin?>(null);

        var pluginFolders = await _bundlePluginInstaller.Install("bundle", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Equal("bundle_folder", pluginFolders.Single());
    }

    [Fact]
    public async Task GivenBundlePluginWithOnePlugin_WhenInstall_ThenPluginIsInstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>
            {
                CreateBundlePlugin("other_bundle", "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "other_plugin", "1.0.0")),
                CreatePlugin("other_plugin2", "2.0.0"),
            });
        SetupBundlePluginMocks("bundle", "1.0.0", 1);

        var pluginFolders =
            await _bundlePluginInstaller.Install("bundle", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Equal(2, pluginFolders.Count);
        Assert.Equal("bundle_folder", pluginFolders[0]);
        Assert.Equal("plugin_folder1", pluginFolders[1]);
    }

    [Fact]
    public async Task GivenBundlePluginWithMultiplePlugins_WhenInstall_ThenPluginsAreInstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>
            {
                CreateBundlePlugin("other_bundle", "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "other_plugin", "1.0.0")),
                CreatePlugin("other_plugin2", "2.0.0"),
            });
        SetupBundlePluginMocks("bundle", "1.0.0", 4,
            new TestBundlePlugin(PluginKind.Linker, "linker", "1.0.0"));

        var pluginFolders =
            await _bundlePluginInstaller.Install("bundle", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Equal(5, pluginFolders.Count);
        Assert.Equal("bundle_folder", pluginFolders[0]);
        Assert.Equal("plugin_folder1", pluginFolders[1]);
        Assert.Equal("plugin_folder2", pluginFolders[2]);
        Assert.Equal("plugin_folder3", pluginFolders[3]);
        Assert.Equal("plugin_folder4", pluginFolders[4]);
    }

    [Fact]
    public async Task GivenBundleWithBundles_WhenInstall_ThenPluginsAreInstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(new List<Models.Plugin>());
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("bundle", "1.0.0"))
            .Returns("url");
        _simplePluginInstallerMock.Setup(x => x.Install("url", "bundle", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("bundle_folder");
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginId1", "1.0.0"))
            .Returns("url_plugin1");
        _simplePluginInstallerMock
            .Setup(x => x.Install("url_plugin1", "pluginId1", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("plugin_folder1");
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginId2", "1.0.0"))
            .Returns("url_plugin2");
        _simplePluginInstallerMock
            .Setup(x => x.Install("url_plugin2", "pluginId2", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("plugin_folder2");
        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_folder"))
            .Returns(CreateBundlePlugin("bundle", "1.0.0",
                new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0")));
        _pluginReaderMock.Setup(x => x.ReadPlugin("plugin_folder1"))
            .Returns(CreateBundlePlugin("pluginId1", "1.0.0",
                new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0")));
        _pluginReaderMock.Setup(x => x.ReadPlugin("plugin_folder2"))
            .Returns(CreatePlugin("pluginId2", "1.0.0", "plugin_folder2"));

        var pluginFolders =
            await _bundlePluginInstaller.Install("bundle", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Equal(3, pluginFolders.Count);
        Assert.Equal("bundle_folder", pluginFolders[0]);
        Assert.Equal("plugin_folder1", pluginFolders[1]);
        Assert.Equal("plugin_folder2", pluginFolders[2]);
    }

    [Fact]
    public async Task GivenBundlePluginWithMultiplePlugins_WhenInstall_ThenOldPluginsAreUninstalled()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins))
            .Returns(new List<Models.Plugin>
            {
                CreateBundlePlugin("bundle", "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "plugin1", "0.0.0")),
                CreatePlugin("plugin1", "0.0.0", "old_plugin_folder1")
            });
        SetupBundlePluginMocks("bundle", "4.0.0", 2,
            new TestBundlePlugin(PluginKind.Linker, "linker", "1.0.0"));

        await _bundlePluginInstaller.Install("bundle", "4.0.0", It.IsAny<CancellationToken>());

        _pluginUninstallerMock.Verify(x => x.Uninstall("path"), Times.Once());
        _pluginUninstallerMock.Verify(x => x.Uninstall("old_plugin_folder1"), Times.Once());
    }

    [Fact]
    public async Task GivenBundlePluginThatFails_WhenInstall_ThenDataIsRemoved()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(new List<Models.Plugin>());
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("bundle", "1.0.0"))
            .Returns("url");
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginId", "1.0.0"))
            .Throws(new Exception());
        _simplePluginInstallerMock.Setup(x => x.Install("url", "bundle", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("bundle_folder");
        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_folder")).Returns(CreateBundlePlugin("bundle", "1.0.0",
            new TestBundlePlugin(PluginKind.Plugin, "pluginId", "1.0.0")));

        await Assert.ThrowsAsync<Exception>(() =>
            _bundlePluginInstaller.Install("bundle", "1.0.0", It.IsAny<CancellationToken>()));

        _pluginUninstallerMock.Verify(x => x.ForceUninstall("bundle_folder"), Times.Once());
        _simplePluginInstallerMock.Verify(
            x => x.Install(It.IsAny<string>(), "pluginId", "1.0.0", It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task GivenBundleWithMultiplePluginsWhereOneFails_WhenInstall_ThenDataIsRemoved()
    {
        _pluginReaderMock.Setup(x => x.ReadPlugins(ConfigFolders.PathToPlugins)).Returns(new List<Models.Plugin>());
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("bundle", "1.0.0"))
            .Returns("url");
        _simplePluginInstallerMock.Setup(x => x.Install("url", "bundle", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("bundle_folder");
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginId1", "1.0.0"))
            .Returns("url_plugin1");
        _simplePluginInstallerMock
            .Setup(x => x.Install("url_plugin1", "pluginId1", "1.0.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync("plugin_folder1");
        _pluginFetcherMock.Setup(x => x.GetPluginUrl("pluginId2", "2.0.0"))
            .Throws(new Exception());
        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_folder"))
            .Returns(CreateBundlePlugin("bundle", "1.0.0",
                new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0"),
                new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "2.0.0"),
                new TestBundlePlugin(PluginKind.Plugin, "pluginId3", "1.0.0")));

        await Assert.ThrowsAsync<Exception>(() =>
            _bundlePluginInstaller.Install("bundle", "1.0.0", It.IsAny<CancellationToken>()));

        _pluginFetcherMock.Verify(
            x => x.GetPluginUrl("pluginId2", "2.0.0"), Times.Once());
        _simplePluginInstallerMock.Verify(
            x => x.Install(It.IsAny<string>(), "pluginId2", "2.0.0", It.IsAny<CancellationToken>()), Times.Never());

        _simplePluginInstallerMock.Verify(
            x => x.Install(It.IsAny<string>(), It.IsAny<string>(), "1.0.0", It.IsAny<CancellationToken>()),
            Times.Between(1, 3, Range.Inclusive));
        _pluginUninstallerMock.Verify(x => x.ForceUninstall(It.IsAny<string>()), Times.Between(1, 3, Range.Inclusive));
    }

    private void SetupBundlePluginMocks(string bundleName, string bundleVersion, int pluginCount,
        params TestBundlePlugin[] bundleExtensionPoints)
    {
        _pluginFetcherMock.Setup(x => x.GetPluginUrl(bundleName, bundleVersion))
            .Returns("url");
        _simplePluginInstallerMock
            .Setup(x => x.Install("url", bundleName, bundleVersion, It.IsAny<CancellationToken>()))
            .ReturnsAsync("bundle_folder");

        var testPlugins = new List<TestBundlePlugin>(bundleExtensionPoints);

        for (var i = 1; i <= pluginCount; i++)
        {
            var pluginId = $"plugin{i}";
            var pluginUrl = $"url{i}";
            testPlugins.Add(new TestBundlePlugin(PluginKind.Plugin, pluginId, "1.0.0"));

            _pluginFetcherMock.Setup(x => x.GetPluginUrl(pluginId, "1.0.0"))
                .Returns(pluginUrl);
            _simplePluginInstallerMock
                .Setup(x => x.Install(pluginUrl, pluginId, "1.0.0", It.IsAny<CancellationToken>()))
                .ReturnsAsync($"plugin_folder{i}");
        }

        _pluginReaderMock.Setup(x => x.ReadPlugin("bundle_folder"))
            .Returns(CreateBundlePlugin("bundle", "1.0.0", testPlugins.ToArray()));
    }

    #endregion
}

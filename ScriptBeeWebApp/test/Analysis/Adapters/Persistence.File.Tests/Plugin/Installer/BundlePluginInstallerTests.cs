using System.Collections;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using OneOf;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Marketplace.Client;
using ScriptBee.Marketplace.Client.Errors;
using ScriptBee.Persistence.File.Plugin.Installer;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Plugins.Installer;
using ScriptBee.Tests.Common.Plugin;
using static ScriptBee.Tests.Common.Plugin.PluginUtils;

namespace ScriptBee.Persistence.File.Tests.Plugin.Installer;

public class BundlePluginInstallerTests
{
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly ISimplePluginInstaller _simplePluginInstaller =
        Substitute.For<ISimplePluginInstaller>();

    private readonly IPluginUninstaller _pluginUninstaller = Substitute.For<IPluginUninstaller>();
    private readonly IPluginUrlFetcher _pluginFetcher = Substitute.For<IPluginUrlFetcher>();

    private readonly ILogger<BundlePluginInstaller> _logger = Substitute.For<
        ILogger<BundlePluginInstaller>
    >();

    private readonly BundlePluginInstaller _bundlePluginInstaller;

    public BundlePluginInstallerTests()
    {
        _bundlePluginInstaller = new BundlePluginInstaller(
            _pluginReader,
            _simplePluginInstaller,
            _pluginUninstaller,
            _pluginFetcher,
            _logger
        );
    }

    #region Simple Plugin Tests

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePlugin_WhenInstall_ThenPluginIsInstalledSuccessfully(
        PluginList pluginList
    )
    {
        _pluginReader.ReadPlugins(ConfigFolders.PathToPlugins).Returns(pluginList.Plugins);
        _pluginFetcher.GetPluginUrl("pluginId", "1.0.0").Returns("url");
        _simplePluginInstaller
            .Install("url", "pluginId", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("plugin_folder");

        var result = await _bundlePluginInstaller.Install(
            "pluginId",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.Single().ShouldBe("plugin_folder");
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndUrlFetchFailsWithPluginNotFound_WhenInstall_ThenReturnsError(
        PluginList pluginList
    )
    {
        _pluginReader.ReadPlugins(ConfigFolders.PathToPlugins).Returns(pluginList.Plugins);
        _pluginFetcher
            .GetPluginUrl("nonExistentPlugin", "1.0.0")
            .Returns(_ => new PluginNotFoundError("nonExistentPlugin"));

        var result = await _bundlePluginInstaller.Install(
            "nonExistentPlugin",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        result.AsT2.ShouldBeEquivalentTo(new PluginInstallationError("nonExistentPlugin", "1.0.0"));
        _pluginUninstaller.Received(0).Uninstall(Arg.Any<string>());
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndUrlFetchFailsWithPluginVersionNotFoundError_WhenInstall_ThenReturnsError(
        PluginList pluginList
    )
    {
        _pluginReader.ReadPlugins(ConfigFolders.PathToPlugins).Returns(pluginList.Plugins);
        _pluginFetcher
            .GetPluginUrl("plugin", "1.0.0")
            .Returns(_ => new PluginVersionNotFoundError("plugin", "1.0.0"));

        var result = await _bundlePluginInstaller.Install(
            "plugin",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        _pluginUninstaller.Received(0).Uninstall(Arg.Any<string>());
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndInstallFails_WhenInstall_ThenReturnsError(
        PluginList pluginList
    )
    {
        _pluginReader.ReadPlugins(ConfigFolders.PathToPlugins).Returns(pluginList.Plugins);
        _pluginFetcher.GetPluginUrl("pluginId", "1.0.0").Returns("url");
        _simplePluginInstaller
            .Install(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromException<
                    OneOf<string, PluginVersionExistsError, PluginInstallationError>
                >(new Exception("Installation failed"))
            );

        var result = await _bundlePluginInstaller.Install(
            "pluginId",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        result.AsT2.ShouldBeEquivalentTo(new PluginInstallationError("pluginId", "1.0.0"));
        _pluginUninstaller.Received(0).Uninstall(Arg.Any<string>());
    }

    [Fact]
    public async Task GivenSimplePluginAlreadyInstalled_WhenInstall_ThenReturnsEmptyList()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(new List<Domain.Model.Plugin.Plugin> { CreatePlugin("pluginName", "1.0.0") });
        _pluginFetcher.GetPluginUrl("pluginName", "1.0.0").Returns("url");

        var result = await _bundlePluginInstaller.Install(
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.ShouldBeEmpty();
        await _simplePluginInstaller
            .Received(0)
            .Install(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
        _pluginUninstaller.Received(0).Uninstall(Arg.Any<string>());
    }

    [Fact]
    public async Task GivenSimplePluginVersionAndExistingNewVersion_WhenInstall_ThenReturnsEmptyList()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(new List<Domain.Model.Plugin.Plugin> { CreatePlugin("pluginName", "10.0.0") });
        _pluginFetcher.GetPluginUrl("pluginName", "1.0.0").Returns("url");

        var result = await _bundlePluginInstaller.Install(
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.ShouldBeEmpty();
        await _simplePluginInstaller
            .Received(0)
            .Install(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenSimplePluginAndExistingOldVersion_WhenInstall_ThenOldVersionsAreUninstalled()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(
                new List<Domain.Model.Plugin.Plugin>
                {
                    CreatePlugin("pluginName", "1.0.0", "plugin_folder_1"),
                    CreatePlugin("pluginName", "14.0.0", "plugin_folder_2"),
                    CreatePlugin("pluginName", "14.3.0", "plugin_folder_3"),
                    CreatePlugin("pluginName", "1.2.3", "plugin_folder_4"),
                    CreatePlugin("pluginName", "1.5.0", "plugin_folder_5"),
                }
            );
        _pluginFetcher.GetPluginUrl("pluginName", "14.3.1").Returns("url");
        _simplePluginInstaller
            .Install("url", "pluginName", "14.3.1", Arg.Any<CancellationToken>())
            .Returns("plugin_folder");

        var result = await _bundlePluginInstaller.Install(
            "pluginName",
            "14.3.1",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.Single().ShouldBe("plugin_folder");
        for (var i = 1; i <= 5; i++)
        {
            _pluginUninstaller.Received(1).Uninstall($"plugin_folder_{i}");
        }
    }

    private static TestPlugin CreatePlugin(string id, string version, string path = "path")
    {
        return new TestPlugin(id, new Version(version), path);
    }

    public sealed record PluginList(List<Domain.Model.Plugin.Plugin> Plugins);

    private class BundlePluginInstallerSimpleTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [new PluginList([])];
            yield return [new PluginList([new TestPlugin("pluginName", new Version(1, 0, 0))])];
            yield return
            [
                new PluginList([
                    new TestPlugin("pluginName", new Version(1, 0, 0)),
                    new TestPlugin("pluginName", new Version(1, 0, 1)),
                ]),
            ];
            yield return
            [
                new PluginList([
                    new TestPlugin("pluginName2", new Version(1, 0, 0)),
                    new TestPlugin("pluginName3", new Version(1, 0, 0)),
                    new TestPlugin("pluginName3", new Version(1, 0, 5)),
                ]),
            ];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion

    #region Bundle Plugin Tests

    [Fact]
    public async Task GivenBundlePluginThatHasNoPluginInFolder_WhenInstall_ThenOnlyFolderIsInstalled()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(new List<Domain.Model.Plugin.Plugin>());
        _pluginFetcher.GetPluginUrl("bundle", "1.0.0").Returns("url");
        _simplePluginInstaller
            .Install("url", "bundle", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("bundle_folder");
        _pluginReader.ReadPlugin("bundle_folder").Returns((Domain.Model.Plugin.Plugin?)null);

        var result = await _bundlePluginInstaller.Install(
            "bundle",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.Single().ShouldBe("bundle_folder");
    }

    [Fact]
    public async Task GivenBundlePluginWithOnePlugin_WhenInstall_ThenPluginIsInstalled()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(
                new List<Domain.Model.Plugin.Plugin>
                {
                    CreateBundlePlugin(
                        "other_bundle",
                        "1.0.0",
                        new TestBundlePlugin(PluginKind.Plugin, "other_plugin", "1.0.0")
                    ),
                    CreatePlugin("other_plugin2", "2.0.0"),
                }
            );
        SetupBundlePlugins("bundle", "1.0.0", 1);

        var result = await _bundlePluginInstaller.Install(
            "bundle",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.Count.ShouldBe(2);
        result.AsT0[0].ShouldBe("bundle_folder");
        result.AsT0[1].ShouldBe("plugin_folder1");
    }

    [Fact]
    public async Task GivenBundlePluginWithMultiplePlugins_WhenInstall_ThenPluginsAreInstalled()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(
                new List<Domain.Model.Plugin.Plugin>
                {
                    CreateBundlePlugin(
                        "other_bundle",
                        "1.0.0",
                        new TestBundlePlugin(PluginKind.Plugin, "other_plugin", "1.0.0")
                    ),
                    CreatePlugin("other_plugin2", "2.0.0"),
                }
            );
        SetupBundlePlugins(
            "bundle",
            "1.0.0",
            4,
            new TestBundlePlugin(PluginKind.Linker, "linker", "1.0.0")
        );

        var result = await _bundlePluginInstaller.Install(
            "bundle",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.Count.ShouldBe(5);
        result.AsT0[0].ShouldBe("bundle_folder");
        result.AsT0[1].ShouldBe("plugin_folder1");
        result.AsT0[2].ShouldBe("plugin_folder2");
        result.AsT0[3].ShouldBe("plugin_folder3");
        result.AsT0[4].ShouldBe("plugin_folder4");
    }

    [Fact]
    public async Task GivenBundleWithBundles_WhenInstall_ThenPluginsAreInstalled()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(new List<Domain.Model.Plugin.Plugin>());
        _pluginFetcher.GetPluginUrl("bundle", "1.0.0").Returns("url");
        _simplePluginInstaller
            .Install("url", "bundle", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("bundle_folder");
        _pluginFetcher.GetPluginUrl("pluginId1", "1.0.0").Returns("url_plugin1");
        _simplePluginInstaller
            .Install("url_plugin1", "pluginId1", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("plugin_folder1");
        _pluginFetcher.GetPluginUrl("pluginId2", "1.0.0").Returns("url_plugin2");
        _simplePluginInstaller
            .Install("url_plugin2", "pluginId2", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("plugin_folder2");
        _pluginReader
            .ReadPlugin("bundle_folder")
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0")
                )
            );
        _pluginReader
            .ReadPlugin("plugin_folder1")
            .Returns(
                CreateBundlePlugin(
                    "pluginId1",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "1.0.0")
                )
            );
        _pluginReader
            .ReadPlugin("plugin_folder2")
            .Returns(CreatePlugin("pluginId2", "1.0.0", "plugin_folder2"));

        var result = await _bundlePluginInstaller.Install(
            "bundle",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.Count.ShouldBe(3);
        result.AsT0[0].ShouldBe("bundle_folder");
        result.AsT0[1].ShouldBe("plugin_folder1");
        result.AsT0[2].ShouldBe("plugin_folder2");
    }

    [Fact]
    public async Task GivenBundlePluginWithMultiplePlugins_WhenInstall_ThenOldPluginsAreUninstalled()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(
                new List<Domain.Model.Plugin.Plugin>
                {
                    CreateBundlePlugin(
                        "bundle",
                        "1.0.0",
                        new TestBundlePlugin(PluginKind.Plugin, "plugin1", "0.0.0")
                    ),
                    CreatePlugin("plugin1", "0.0.0", "old_plugin_folder1"),
                }
            );
        SetupBundlePlugins(
            "bundle",
            "4.0.0",
            2,
            new TestBundlePlugin(PluginKind.Linker, "linker", "1.0.0")
        );

        var result = await _bundlePluginInstaller.Install(
            "bundle",
            "4.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        _pluginUninstaller.Received(1).Uninstall("path");
        _pluginUninstaller.Received(1).Uninstall("old_plugin_folder1");
    }

    [Fact]
    public async Task GivenBundlePluginThatFails_WhenInstall_ThenReturnsErrorAndDataIsRemoved()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(new List<Domain.Model.Plugin.Plugin>());
        _pluginFetcher.GetPluginUrl("bundle", "1.0.0").Returns("url");
        _pluginFetcher
            .GetPluginUrl("pluginId", "1.0.0")
            .Returns(
                (OneOf<string, PluginNotFoundError, PluginVersionNotFoundError>)
                    new PluginNotFoundError("pluginId")
            );
        _simplePluginInstaller
            .Install("url", "bundle", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("bundle_folder");
        _pluginReader
            .ReadPlugin("bundle_folder")
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId", "1.0.0")
                )
            );

        var result = await _bundlePluginInstaller.Install(
            "bundle",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        result.AsT2.ShouldBeEquivalentTo(new PluginInstallationError("bundle", "1.0.0"));
        _pluginUninstaller.Received(1).ForceUninstall("bundle_folder");
        await _simplePluginInstaller
            .Received(0)
            .Install(Arg.Any<string>(), "pluginId", "1.0.0", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenBundleWithMultiplePluginsWhereOneFails_WhenInstall_ThenReturnsErrorAndDataIsRemoved()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(new List<Domain.Model.Plugin.Plugin>());
        _pluginFetcher.GetPluginUrl("bundle", "1.0.0").Returns("url");
        _simplePluginInstaller
            .Install("url", "bundle", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("bundle_folder");
        _pluginFetcher.GetPluginUrl("pluginId1", "1.0.0").Returns("url_plugin1");
        _simplePluginInstaller
            .Install("url_plugin1", "pluginId1", "1.0.0", Arg.Any<CancellationToken>())
            .Returns("plugin_folder1");
        _pluginFetcher
            .GetPluginUrl("pluginId2", "2.0.0")
            .Returns(_ => new PluginVersionNotFoundError("pluginId2", "2.0.0"));
        _pluginReader
            .ReadPlugin("bundle_folder")
            .Returns(
                CreateBundlePlugin(
                    "bundle",
                    "1.0.0",
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId1", "1.0.0"),
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId2", "2.0.0"),
                    new TestBundlePlugin(PluginKind.Plugin, "pluginId3", "1.0.0")
                )
            );

        var result = await _bundlePluginInstaller.Install(
            "bundle",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        result.AsT2.ShouldBeEquivalentTo(new PluginInstallationError("bundle", "1.0.0"));
        _pluginFetcher.Received(1).GetPluginUrl("pluginId2", "2.0.0");
        await _simplePluginInstaller
            .Received(0)
            .Install(Arg.Any<string>(), "pluginId2", "2.0.0", Arg.Any<CancellationToken>());
        await _simplePluginInstaller
            .Received(Quantity.Within(1, 3))
            .Install(Arg.Any<string>(), Arg.Any<string>(), "1.0.0", Arg.Any<CancellationToken>());
        _pluginUninstaller.Received(Quantity.Within(1, 3)).ForceUninstall(Arg.Any<string>());
    }

    private void SetupBundlePlugins(
        string bundleName,
        string bundleVersion,
        int pluginCount,
        params TestBundlePlugin[] bundleExtensionPoints
    )
    {
        _pluginFetcher.GetPluginUrl(bundleName, bundleVersion).Returns("url");
        _simplePluginInstaller
            .Install("url", bundleName, bundleVersion, Arg.Any<CancellationToken>())
            .Returns("bundle_folder");

        var testPlugins = new List<TestBundlePlugin>(bundleExtensionPoints);

        for (var i = 1; i <= pluginCount; i++)
        {
            var pluginId = $"plugin{i}";
            var pluginUrl = $"url{i}";
            testPlugins.Add(new TestBundlePlugin(PluginKind.Plugin, pluginId, "1.0.0"));

            _pluginFetcher.GetPluginUrl(pluginId, "1.0.0").Returns(pluginUrl);
            _simplePluginInstaller
                .Install(pluginUrl, pluginId, "1.0.0", Arg.Any<CancellationToken>())
                .Returns($"plugin_folder{i}");
        }

        _pluginReader
            .ReadPlugin("bundle_folder")
            .Returns(CreateBundlePlugin("bundle", "1.0.0", testPlugins.ToArray()));
    }

    #endregion
}

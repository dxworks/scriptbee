using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Installer;
using ScriptBee.Plugins.Loader;
using ScriptBee.Service.Gateway.Plugins;
using ScriptBee.Tests.Common;
using ScriptBee.Tests.Common.Plugins;

namespace ScriptBee.Service.Gateway.Tests.Plugins;

public class PluginManagerTests : IClassFixture<TempDirFixture>
{
    private readonly TempDirFixture _tempDirFixture;
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();
    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();

    private readonly IGatewayPluginPathProvider _pluginPathProvider =
        Substitute.For<IGatewayPluginPathProvider>();

    private readonly IBundlePluginInstaller _bundlePluginInstaller =
        Substitute.For<IBundlePluginInstaller>();

    private readonly ILogger<PluginManager> _logger = Substitute.For<ILogger<PluginManager>>();

    private readonly PluginManager _pluginManager;

    public PluginManagerTests(TempDirFixture tempDirFixture)
    {
        _tempDirFixture = tempDirFixture;
        _pluginManager = new PluginManager(
            _pluginReader,
            _pluginLoader,
            _bundlePluginInstaller,
            _pluginPathProvider,
            _logger
        );
    }

    [Fact]
    public void GivenEmptyPlugins_WhenLoadPlugins_ThenNoPluginsLoaded()
    {
        _pluginPathProvider.GetInstallationFolderPath().Returns("plugin/path");
        _pluginReader.ReadPlugins("plugin/path").Returns(new List<Plugin>());

        _pluginManager.LoadPlugins();

        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }

    [Fact]
    public void GivenAllValidPlugins_WhenLoadPlugins_ThenAllPluginsAreLoaded()
    {
        _pluginPathProvider.GetInstallationFolderPath().Returns("plugin/path");
        _pluginReader
            .ReadPlugins("plugin/path")
            .Returns(
                new List<Plugin>
                {
                    new TestPlugin(new PluginId("id", new Version(0, 0, 0, 1))),
                    new TestPlugin(new PluginId("id", new Version(0, 0, 0, 2))),
                    new TestPlugin(new PluginId("id", new Version(0, 0, 0, 3))),
                }
            );

        _pluginManager.LoadPlugins();

        _pluginLoader.Received(3).Load(Arg.Any<Plugin>());
    }

    [Fact]
    public void GivenSomeInvalidPlugins_WhenLoadPlugins_ThenAllValidPluginsAreLoaded()
    {
        var expectedException = new Exception("Test exception");

        Plugin testPlugin1 = new TestPlugin(new PluginId("id", new Version(0, 0, 0, 1)));
        Plugin testPlugin2 = new TestPlugin(new PluginId("id", new Version(0, 0, 1, 1)));
        Plugin testPlugin3 = new TestPlugin(new PluginId("id", new Version(0, 0, 2, 1)));

        _pluginPathProvider.GetInstallationFolderPath().Returns("plugin/path");
        _pluginReader
            .ReadPlugins("plugin/path")
            .Returns(new List<Plugin> { testPlugin1, testPlugin2, testPlugin3 });
        _pluginLoader.When(x => x.Load(testPlugin2)).Throw(expectedException);

        _pluginManager.LoadPlugins();

        _logger
            .ReceivedWithAnyArgs()
            .LogError(expectedException, "Failed to load plugin {Plugin}", testPlugin2);
    }

    [Fact]
    public void GivenInstalledPlugins_WhenGetInstalledPlugins_ThenReturnCorrectPluginIds()
    {
        _pluginPathProvider.GetInstallationFolderPath().Returns("active/path");
        _pluginReader
            .ReadPlugins("active/path")
            .Returns(
                new List<Plugin>
                {
                    new TestPlugin(new PluginId("id1", new Version(1, 0, 0))),
                    new TestPlugin(new PluginId("id2", new Version(2, 0, 0))),
                }
            );

        var result = _pluginManager.GetInstalledPlugins();

        result.ShouldBe(
            new List<PluginId>
            {
                new("id1", new Version(1, 0, 0)),
                new("id2", new Version(2, 0, 0)),
            }
        );
    }

    [Fact]
    public void GivenPluginId_WhenUninstall_ThenPluginUnloadedAndDeleted()
    {
        var pluginId = new PluginId("id", new Version(1, 0, 0));
        _pluginPathProvider.GetInstallationFolderPath().Returns("active/path");

        _pluginManager.Uninstall(pluginId);

        _pluginLoader.Received(1).Unload(pluginId);
    }

    [Fact]
    public async Task GivenInstallerReturnsMultiplePlugins_WhenInstall_ThenAllAreCopiedAndLoaded()
    {
        // Arrange
        var pluginId1 = new PluginId("id1", new Version(1, 0, 0));
        var pluginId2 = new PluginId("id2", new Version(2, 0, 0));
        _bundlePluginInstaller
            .Install(pluginId1, TestContext.Current.CancellationToken)
            .Returns(new List<PluginId> { pluginId1, pluginId2 });

        var cache = _tempDirFixture.CreateSubFolder("cache");
        var active = _tempDirFixture.CreateSubFolder("active");

        Directory.CreateDirectory(Path.Combine(cache, pluginId1.GetFullyQualifiedName()));
        Directory.CreateDirectory(Path.Combine(cache, pluginId2.GetFullyQualifiedName()));

        _pluginPathProvider.GetPathToPlugins().Returns(cache);
        _pluginPathProvider.GetInstallationFolderPath().Returns(active);
        _pluginReader.ReadPlugin(Arg.Any<string>()).Returns(new TestPlugin(pluginId1));

        // Act
        await _pluginManager.Install(pluginId1, TestContext.Current.CancellationToken);

        // Assert
        _pluginLoader.Received(2).Load(Arg.Any<Plugin>());
        Directory.Exists(Path.Combine(active, pluginId1.GetFullyQualifiedName())).ShouldBeTrue();
        Directory.Exists(Path.Combine(active, pluginId2.GetFullyQualifiedName())).ShouldBeTrue();
    }

    [Fact]
    public async Task GivenCacheFolderDoesNotExist_WhenInstall_ThenPluginIsNotCopiedOrLoaded()
    {
        var pluginId = new PluginId("id", new Version(1, 0, 0));
        _bundlePluginInstaller
            .Install(pluginId, TestContext.Current.CancellationToken)
            .Returns(new List<PluginId> { pluginId });

        _pluginPathProvider.GetPathToPlugins().Returns("non-existent-cache");
        _pluginPathProvider.GetInstallationFolderPath().Returns("active");

        await _pluginManager.Install(pluginId, TestContext.Current.CancellationToken);

        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }
}

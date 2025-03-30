using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Plugins.Installer;

namespace ScriptBee.Service.Plugin.Tests;

public class InstallPluginServiceTest
{
    private readonly IBundlePluginInstaller _bundlePluginInstaller =
        Substitute.For<IBundlePluginInstaller>();

    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly ILogger<UninstallPluginService> _logger = Substitute.For<
        ILogger<UninstallPluginService>
    >();

    private readonly InstallPluginService _installPluginService;

    public InstallPluginServiceTest()
    {
        _installPluginService = new InstallPluginService(
            _bundlePluginInstaller,
            _pluginLoader,
            _pluginReader,
            _logger
        );
    }

    [Fact]
    public async Task InstallPlugin_ReadsAndLoadsEachInstalledPlugin()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installPaths = new List<string> { "path1", "path2" };
        var plugin1 = new Domain.Model.Plugin.Plugin(
            "folder",
            "plugin-1",
            new Version(),
            new PluginManifest()
        );
        var plugin2 = new Domain.Model.Plugin.Plugin(
            "folder",
            "plugin-2",
            new Version(),
            new PluginManifest()
        );
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(installPaths);
        _pluginReader.ReadPlugin("path1").Returns(plugin1);
        _pluginReader.ReadPlugin("path2").Returns(plugin2);

        await _installPluginService.InstallPlugin(pluginId, version);

        _pluginReader.Received(1).ReadPlugin("path1");
        _pluginReader.Received(1).ReadPlugin("path2");
        _pluginLoader.Received(1).Load(plugin1);
        _pluginLoader.Received(1).Load(plugin2);
    }

    [Fact]
    public async Task InstallPlugin_LogsWarningAndContinues_WhenPluginReaderReturnsNull()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installPaths = new List<string> { "path1", "path2" };
        var plugin2 = new Domain.Model.Plugin.Plugin(
            "folder",
            "plugin-2",
            new Version(),
            new PluginManifest()
        );
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(installPaths);
        _pluginReader.ReadPlugin("path1").Returns((Domain.Model.Plugin.Plugin?)null);
        _pluginReader.ReadPlugin("path2").Returns(plugin2);

        await _installPluginService.InstallPlugin(pluginId, version);

        _logger
            .Received(1)
            .ReceivedWithAnyArgs()
            .LogWarning("Plugin Manifest from {Path} could not be read", "path1");
        _pluginLoader.Received(1).Load(plugin2);
    }

    [Fact]
    public async Task InstallPlugin_DoesNotCallPluginLoader_WhenPluginReaderReturnsNull()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installPaths = new List<string> { "path1" };
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(installPaths);
        _pluginReader.ReadPlugin("path1").Returns((Domain.Model.Plugin.Plugin?)null);

        await _installPluginService.InstallPlugin(pluginId, version);

        _pluginLoader.DidNotReceive().Load(Arg.Any<Domain.Model.Plugin.Plugin>());
    }

    [Fact]
    public async Task InstallPlugin_HandlesEmptyInstallPaths()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        _bundlePluginInstaller.Install(pluginId, version, Arg.Any<CancellationToken>()).Returns([]);

        await _installPluginService.InstallPlugin(pluginId, version);

        _pluginReader.DidNotReceive().ReadPlugin(Arg.Any<string>());
        _pluginLoader.DidNotReceive().Load(Arg.Any<Domain.Model.Plugin.Plugin>());
    }
}

using Microsoft.Extensions.Logging;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Installer;
using ScriptBee.Plugins.Loader;
using PluginInstallationError = ScriptBee.Plugins.Installer.PluginInstallationError;
using PluginVersionExistsError = ScriptBee.Plugins.Installer.PluginVersionExistsError;

namespace ScriptBee.Service.Plugin.Tests;

public class InstallPluginServiceTest
{
    private readonly IBundlePluginInstaller _bundlePluginInstaller =
        Substitute.For<IBundlePluginInstaller>();

    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly ILogger<InstallPluginService> _logger = Substitute.For<
        ILogger<InstallPluginService>
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
    public async Task GivenValidInstallPaths_WhenInstallPlugin_ThenReadsAndLoadsEachPlugin()
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
            .Returns(
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(installPaths)
            );
        _pluginReader.ReadPlugin("path1").Returns(plugin1);
        _pluginReader.ReadPlugin("path2").Returns(plugin2);

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        _pluginReader.Received(1).ReadPlugin("path1");
        _pluginReader.Received(1).ReadPlugin("path2");
        _pluginLoader.Received(1).Load(plugin1);
        _pluginLoader.Received(1).Load(plugin2);
    }

    [Fact]
    public async Task GivenValidInstallPaths_WhenInstallPlugin_ThenReturnsSuccess()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installPaths = new List<string> { "path1" };
        var plugin1 = new Domain.Model.Plugin.Plugin(
            "folder",
            "plugin-1",
            new Version(),
            new PluginManifest()
        );
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(installPaths)
            );
        _pluginReader.ReadPlugin("path1").Returns(plugin1);

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenPluginReaderReturnsNull_WhenInstallPlugin_ThenLogsWarningAndContinues()
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
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(installPaths)
            );
        _pluginReader.ReadPlugin("path1").Returns((Domain.Model.Plugin.Plugin?)null);
        _pluginReader.ReadPlugin("path2").Returns(plugin2);

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        _logger
            .Received(1)
            .ReceivedWithAnyArgs()
            .LogWarning("Plugin Manifest from {Path} could not be read", "path1");
        _pluginLoader.Received(1).Load(plugin2);
        _pluginLoader.Received(0).Load(Arg.Is<Domain.Model.Plugin.Plugin>(p => p.Id == "plugin-1"));
    }

    [Fact]
    public async Task GivenAllPluginsReadingFails_WhenInstallPlugin_ThenDoesNotLoadAny()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installPaths = new List<string> { "path1", "path2" };
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(installPaths)
            );
        _pluginReader.ReadPlugin("path1").Returns((Domain.Model.Plugin.Plugin?)null);
        _pluginReader.ReadPlugin("path2").Returns((Domain.Model.Plugin.Plugin?)null);

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        _pluginLoader.DidNotReceive().Load(Arg.Any<Domain.Model.Plugin.Plugin>());
        _logger
            .Received(1)
            .ReceivedWithAnyArgs()
            .LogWarning("Plugin Manifest from path1 could not be read");
        _logger
            .Received(1)
            .ReceivedWithAnyArgs()
            .LogWarning("Plugin Manifest from path2 could not be read");
    }

    [Fact]
    public async Task GivenSinglePluginReadsNull_WhenInstallPlugin_ThenDoesNotLoad()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installPaths = new List<string> { "path1" };
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(installPaths)
            );
        _pluginReader.ReadPlugin("path1").Returns((Domain.Model.Plugin.Plugin?)null);

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        _pluginLoader.DidNotReceive().Load(Arg.Any<Domain.Model.Plugin.Plugin>());
    }

    [Fact]
    public async Task GivenEmptyInstallPaths_WhenInstallPlugin_ThenReturnsSuccess()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(new List<string>())
            );

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        _pluginReader.DidNotReceive().ReadPlugin(Arg.Any<string>());
        _pluginLoader.DidNotReceive().Load(Arg.Any<Domain.Model.Plugin.Plugin>());
    }

    [Fact]
    public async Task GivenPluginVersionExistsError_WhenInstallPlugin_ThenReturnsInvalidPluginError()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var versionExistsError = new PluginVersionExistsError(pluginId, version);
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(versionExistsError)
            );

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
        var error = result.AsT1;
        error.Name.ShouldBe(pluginId);
        error.Version.ShouldBe(version);
        _pluginLoader.DidNotReceive().Load(Arg.Any<Domain.Model.Plugin.Plugin>());
    }

    [Fact]
    public async Task GivenPluginInstallationError_WhenInstallPlugin_ThenReturnsPluginInstallationError()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installationError = new PluginInstallationError(pluginId, version);
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(installationError)
            );

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBeTrue();
        var error = result.AsT2;
        error.Name.ShouldBe(pluginId);
        error.Version.ShouldBe(version);
        _pluginLoader.DidNotReceive().Load(Arg.Any<Domain.Model.Plugin.Plugin>());
        _pluginReader.DidNotReceive().ReadPlugin(Arg.Any<string>());
    }

    [Fact]
    public async Task GivenMultiplePluginsWithMixedResults_WhenInstallPlugin_ThenLoadsSuccessfulAndLogsFailures()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var installPaths = new List<string> { "path1", "path2", "path3", "path4" };
        var plugin1 = new Domain.Model.Plugin.Plugin(
            "folder",
            "plugin-1",
            new Version(),
            new PluginManifest()
        );
        var plugin3 = new Domain.Model.Plugin.Plugin(
            "folder",
            "plugin-3",
            new Version(),
            new PluginManifest()
        );
        _bundlePluginInstaller
            .Install(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(_ =>
                Task.FromResult<
                    OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>
                >(installPaths)
            );
        _pluginReader.ReadPlugin("path1").Returns(plugin1);
        _pluginReader.ReadPlugin("path2").Returns((Domain.Model.Plugin.Plugin?)null);
        _pluginReader.ReadPlugin("path3").Returns(plugin3);
        _pluginReader.ReadPlugin("path4").Returns((Domain.Model.Plugin.Plugin?)null);

        var result = await _installPluginService.InstallPlugin(
            pluginId,
            version,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        _pluginLoader.Received(1).Load(plugin1);
        _pluginLoader.Received(1).Load(plugin3);
        _pluginLoader.Received(2).Load(Arg.Any<Domain.Model.Plugin.Plugin>());
        _logger
            .Received(1)
            .ReceivedWithAnyArgs()
            .LogWarning("Plugin Manifest from path2 could not be read");
        _logger
            .Received(1)
            .ReceivedWithAnyArgs()
            .LogWarning("Plugin Manifest from path4 could not be read");
    }
}

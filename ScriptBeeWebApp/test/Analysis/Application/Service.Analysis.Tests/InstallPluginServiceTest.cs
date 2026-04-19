using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OneOf.Types;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis.Errors;

namespace ScriptBee.Analysis.Service.Tests;

public class InstallPluginServiceTest
{
    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly InstallPluginService _installPluginService;

    public InstallPluginServiceTest()
    {
        _installPluginService = new InstallPluginService(
            _pluginReader,
            _pluginLoader,
            _pluginPathProvider
        );
    }

    [Fact]
    public void GivenPluginReadSuccessFully_ExpectPluginToBeLoaded()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        var plugin = new Plugin("folder", "plugin-1", new Version(), new PluginManifest());
        _pluginPathProvider.GetPathToPlugins().Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId, version).Returns(plugin);

        var result = _installPluginService.InstallPlugin(pluginId, version);

        result.AsT0.ShouldBe(new Success());
        _pluginLoader.Received(1).Load(plugin);
    }

    [Fact]
    public void GivenPluginReadReturnsNull_ExpectInvalidPluginError()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        _pluginPathProvider.GetPathToPlugins().Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId, version).Returns((Plugin?)null);

        var result = _installPluginService.InstallPlugin(pluginId, version);

        result.AsT1.ShouldBe(new InvalidPluginError(pluginId, version));
        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }

    [Fact]
    public void GivenException_ExpectPluginInstallationError()
    {
        const string pluginId = "testPlugin";
        const string version = "1.0.0";
        _pluginPathProvider.GetPathToPlugins().Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId, version).Throws(new Exception());

        var result = _installPluginService.InstallPlugin(pluginId, version);

        result.AsT2.ShouldBe(new PluginInstallationError(pluginId, version));
        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }
}

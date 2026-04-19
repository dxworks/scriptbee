using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OneOf.Types;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
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
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));
        var plugin = new Plugin("folder", pluginId, new PluginManifest());
        _pluginPathProvider.GetPathToPlugins().Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId).Returns(plugin);

        var result = _installPluginService.InstallPlugin(pluginId);

        result.AsT0.ShouldBe(new Success());
        _pluginLoader.Received(1).Load(plugin);
    }

    [Fact]
    public void GivenPluginReadReturnsNull_ExpectInvalidPluginError()
    {
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));
        _pluginPathProvider.GetPathToPlugins().Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId).Returns((Plugin?)null);

        var result = _installPluginService.InstallPlugin(pluginId);

        result.AsT1.ShouldBe(new InvalidPluginError(pluginId));
        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }

    [Fact]
    public void GivenException_ExpectPluginInstallationError()
    {
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));
        _pluginPathProvider.GetPathToPlugins().Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId).Throws(new Exception());

        var result = _installPluginService.InstallPlugin(pluginId);

        result.AsT2.ShouldBe(new PluginInstallationError(pluginId));
        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }
}

using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OneOf.Types;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis.Errors;

namespace ScriptBee.Analysis.Service.Tests;

public class InstallPluginServiceTest
{
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();
    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly InstallPluginService _installPluginService;

    public InstallPluginServiceTest()
    {
        _installPluginService = new InstallPluginService(
            _projectManager,
            _pluginReader,
            _pluginLoader,
            _pluginPathProvider
        );
    }

    [Fact]
    public void GivenPluginReadSuccessFullyFromProjectFolder_ExpectPluginToBeLoaded()
    {
        // Arrange
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));
        var plugin = new Plugin("folder", pluginId, new PluginManifest());
        var projectId = ProjectId.FromValue("project1");

        _projectManager.GetProject().Returns(new Project { Id = projectId.ToString() });
        _pluginPathProvider.GetPathToPlugins(projectId).Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId).Returns(plugin);

        // Act
        var result = _installPluginService.InstallPlugin(pluginId);

        // Assert
        result.AsT0.ShouldBe(new Success());
        _pluginLoader.Received(1).Load(plugin);
    }

    [Fact]
    public void GivenPluginReadSuccessFullyFromGlobalFolder_ExpectPluginToBeLoaded()
    {
        // Arrange
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));
        var plugin = new Plugin("folder", pluginId, new PluginManifest());
        var projectId = ProjectId.FromValue("project1");

        _projectManager.GetProject().Returns(new Project { Id = projectId.ToString() });
        _pluginPathProvider.GetPathToPlugins(projectId).Returns("project-plugins-path");
        _pluginPathProvider.GetPathToPlugins().Returns("global-plugins-path");
        _pluginReader.ReadPlugin("project-plugins-path", pluginId).Returns((Plugin?)null);
        _pluginReader.ReadPlugin("global-plugins-path", pluginId).Returns(plugin);

        // Act
        var result = _installPluginService.InstallPlugin(pluginId);

        // Assert
        result.AsT0.ShouldBe(new Success());
        _pluginLoader.Received(1).Load(plugin);
    }

    [Fact]
    public void GivenPluginReadReturnsNull_ExpectInvalidPluginError()
    {
        // Arrange
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));
        var projectId = ProjectId.FromValue("project1");

        _projectManager.GetProject().Returns(new Project { Id = projectId.ToString() });
        _pluginPathProvider.GetPathToPlugins(projectId).Returns("plugins-path");
        _pluginReader.ReadPlugin("plugins-path", pluginId).Returns((Plugin?)null);
        _pluginPathProvider.GetPathToPlugins().Returns("global-plugins-path");
        _pluginReader.ReadPlugin("global-plugins-path", pluginId).Returns((Plugin?)null);

        // Act
        var result = _installPluginService.InstallPlugin(pluginId);

        // Assert
        result.AsT1.ShouldBe(new InvalidPluginError(pluginId));
        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }

    [Fact]
    public void GivenException_ExpectPluginInstallationError()
    {
        // Arrange
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));
        var projectId = ProjectId.FromValue("project1");

        _projectManager.GetProject().Returns(new Project { Id = projectId.ToString() });
        _pluginPathProvider.GetPathToPlugins(projectId).Throws(new Exception());

        // Act
        var result = _installPluginService.InstallPlugin(pluginId);

        // Assert
        result.AsT2.ShouldBe(new PluginInstallationError(pluginId));
        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }
}

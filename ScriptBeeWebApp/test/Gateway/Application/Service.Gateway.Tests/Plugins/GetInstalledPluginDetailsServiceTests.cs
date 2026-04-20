using NSubstitute;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Tests.Plugins;

public class GetInstalledPluginDetailsServiceTests
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly GetInstalledPluginDetailsService _service;

    public GetInstalledPluginDetailsServiceTests()
    {
        _service = new GetInstalledPluginDetailsService(
            _getProject,
            _pluginPathProvider,
            _pluginReader
        );
    }

    [Fact]
    public async Task Get_ReturnsPlugin_WhenPluginExistsOnDisk()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project1");
        const string pluginId = "test-plugin";
        var version = new Version("1.2.3");

        var project = new ProjectDetails(
            projectId,
            "Project 1",
            DateTimeOffset.Now,
            new Dictionary<string, List<FileData>>(),
            new Dictionary<string, List<FileData>>(),
            [],
            [new PluginInstallationConfig(pluginId, version)]
        );
        _getProject.GetById(projectId, Arg.Any<CancellationToken>()).Returns(project);

        const string pluginsPath = "plugins";
        _pluginPathProvider.GetPathToPlugins(projectId).Returns(pluginsPath);

        var manifest = new PluginManifest
        {
            Name = "Display Name",
            Description = "Description",
            Author = "Author",
        };
        var loadedPlugin = new Plugin("path", new PluginId(pluginId, version), manifest);

        var expectedPath = Path.Combine(pluginsPath, $"{pluginId}@{version}");
        _pluginReader.ReadPlugin(expectedPath).Returns(loadedPlugin);

        // Act
        var result = await _service.Get(projectId, pluginId, TestContext.Current.CancellationToken);

        // Assert
        result.IsT0.ShouldBeTrue();
        var marketPlugin = result.AsT0;
        marketPlugin.Id.ShouldBe(pluginId);
        marketPlugin.Name.ShouldBe(manifest.Name);
        marketPlugin.Description.ShouldBe(manifest.Description);
        marketPlugin.Authors.ShouldContain(manifest.Author);
        marketPlugin.Versions.ShouldHaveSingleItem().Version.ShouldBe(version);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundError_WhenPluginNotInProject()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project1");
        var project = new ProjectDetails(
            projectId,
            "Project 1",
            DateTimeOffset.Now,
            new Dictionary<string, List<FileData>>(),
            new Dictionary<string, List<FileData>>(),
            [],
            []
        );
        _getProject.GetById(projectId, Arg.Any<CancellationToken>()).Returns(project);

        // Act
        var result = await _service.Get(
            projectId,
            "non-existent",
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT1.ShouldBeTrue();
        result.AsT1.ShouldBeOfType<PluginNotFoundError>();
    }
}

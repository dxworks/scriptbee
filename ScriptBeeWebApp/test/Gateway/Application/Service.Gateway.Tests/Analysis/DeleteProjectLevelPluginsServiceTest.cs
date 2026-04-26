using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Installer;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.Tests.Common;

namespace ScriptBee.Service.Gateway.Tests.Analysis;

public class DeleteProjectLevelPluginsServiceTest : IClassFixture<TempDirFixture>
{
    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IBundlePluginUninstaller _pluginUninstaller =
        Substitute.For<IBundlePluginUninstaller>();

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly ILogger<DeleteProjectLevelPluginsService> _logger = Substitute.For<
        ILogger<DeleteProjectLevelPluginsService>
    >();

    private readonly DeleteProjectLevelPluginsService _service;
    private readonly TempDirFixture _tempDirFixture;

    public DeleteProjectLevelPluginsServiceTest(TempDirFixture tempDirFixture)
    {
        _tempDirFixture = tempDirFixture;
        _service = new DeleteProjectLevelPluginsService(
            _getAllProjectInstances,
            _pluginUninstaller,
            _pluginPathProvider,
            _logger
        );
    }

    [Fact]
    public async Task GivenActiveInstances_WhenDeleteProjectPlugins_ShouldSkipDeletion()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project1");
        var projectDetails = CreateProjectDetails(projectId, []);

        _getAllProjectInstances
            .GetAll(projectId, TestContext.Current.CancellationToken)
            .Returns([InstanceInfoFixture.BasicInstanceInfo(projectId)]);

        // Act
        await _service.DeleteProjectPlugins(projectDetails, TestContext.Current.CancellationToken);

        // Assert
        _pluginPathProvider.DidNotReceive().GetPathToPlugins(Arg.Any<ProjectId>());
        _pluginUninstaller
            .DidNotReceiveWithAnyArgs()
            .Uninstall(Arg.Any<PluginId>(), Arg.Any<string>());
    }

    [Fact]
    public async Task GivenNoActiveInstances_AndUnusedPlugins_WhenDeleteProjectPlugins_ShouldDeleteUnusedPlugins()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project2");
        var tempPath = _tempDirFixture.CreateSubFolder(Guid.NewGuid().ToString());
        var plugin1Path = Path.Combine(tempPath, "plugin1@1.0.0");
        var plugin2Path = Path.Combine(tempPath, "plugin2@2.0.0");
        Directory.CreateDirectory(plugin1Path);
        Directory.CreateDirectory(plugin2Path);

        var projectDetails = CreateProjectDetails(
            projectId,
            [new PluginInstallationConfig("plugin1", new Version(1, 0, 0))]
        );

        _getAllProjectInstances
            .GetAll(projectId, TestContext.Current.CancellationToken)
            .Returns([]);
        _pluginPathProvider.GetPathToPlugins(projectId).Returns(tempPath);

        // Act
        await _service.DeleteProjectPlugins(projectDetails, TestContext.Current.CancellationToken);

        // Assert
        _pluginUninstaller
            .Received(1)
            .Uninstall(new PluginId("plugin2", new Version(2, 0, 0)), tempPath);
        _pluginUninstaller
            .DidNotReceive()
            .Uninstall(new PluginId("plugin1", new Version(1, 0, 0)), Arg.Any<string>());
    }

    [Fact]
    public async Task GivenNoActiveInstances_AndInvalidPluginFolderName_WhenDeleteProjectPlugins_ShouldSkipThatFolder()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project3");
        var tempPath = _tempDirFixture.CreateSubFolder(Guid.NewGuid().ToString());
        var invalidPath = Path.Combine(tempPath, "invalid-folder-name");
        Directory.CreateDirectory(invalidPath);

        var projectDetails = CreateProjectDetails(projectId, []);

        _getAllProjectInstances
            .GetAll(projectId, TestContext.Current.CancellationToken)
            .Returns([]);
        _pluginPathProvider.GetPathToPlugins(projectId).Returns(tempPath);

        // Act
        await _service.DeleteProjectPlugins(projectDetails, TestContext.Current.CancellationToken);

        // Assert
        _pluginUninstaller
            .DidNotReceiveWithAnyArgs()
            .Uninstall(Arg.Any<PluginId>(), Arg.Any<string>());
    }

    private static ProjectDetails CreateProjectDetails(
        ProjectId projectId,
        IEnumerable<PluginInstallationConfig> installedPlugins
    )
    {
        return new ProjectDetails(
            projectId,
            "ProjectName",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>(),
            new Dictionary<string, List<FileData>>(),
            [],
            installedPlugins
        );
    }
}

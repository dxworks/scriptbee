using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Plugin;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Plugin;

namespace ScriptBee.Service.Project.Tests.Plugin;

public class UninstallPluginServiceTests
{
    private readonly IUninstallPlugin _uninstallPlugin = Substitute.For<IUninstallPlugin>();
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly UninstallPluginService _uninstallPluginService;

    public UninstallPluginServiceTests()
    {
        _uninstallPluginService = new UninstallPluginService(
            _uninstallPlugin,
            _getProject,
            _getAllProjectInstances,
            _updateProject
        );
    }

    [Fact]
    public async Task GivenNoProject_ExpectProjectDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var result = await _uninstallPluginService.UninstallPluginAsync(
            new UninstallPluginCommand(projectId, "pluginId", "1.2.3"),
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
    }

    [Theory]
    [InlineData("pluginId", "0.5.1")]
    [InlineData("other-plugin", "1.2.3")]
    [InlineData("other-plugin", "5.3.2")]
    public async Task GivenPluginVersionNotInstalled_ExpectNoInteractions(
        string pluginId,
        string pluginVersion
    )
    {
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId) with
        {
            InstalledPlugins = [new PluginInstallationConfig(pluginId, pluginVersion)],
        };
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        var result = await _uninstallPluginService.UninstallPluginAsync(
            new UninstallPluginCommand(projectId, "pluginId", "1.2.3"),
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(projectDetails);
        await _uninstallPlugin
            .Received(0)
            .Uninstall(
                Arg.Any<InstanceInfo>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
        await _updateProject
            .Received(0)
            .Update(Arg.Any<ProjectDetails>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInstances_ExpectPluginUninstalledToAllInstances()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId) with
        {
            InstalledPlugins =
            [
                new PluginInstallationConfig("pluginId", "1.2.3"),
                new PluginInstallationConfig("other", "1.5.1"),
            ],
        };
        var updatedProjectDetails = projectDetails with
        {
            InstalledPlugins = [new PluginInstallationConfig("other", "1.5.1")],
        };
        var instanceInfo = InstanceInfoFixture.BasicInstanceInfo(projectId);

        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _getAllProjectInstances
            .GetAll(projectId, Arg.Any<CancellationToken>())
            .Returns(new List<InstanceInfo> { instanceInfo });
        _updateProject
            .Update(Arg.Any<ProjectDetails>(), Arg.Any<CancellationToken>())
            .Returns(updatedProjectDetails);

        // Act
        var result = await _uninstallPluginService.UninstallPluginAsync(
            new UninstallPluginCommand(projectId, "pluginId", "1.2.3"),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.InstalledPlugins.ShouldBe(updatedProjectDetails.InstalledPlugins);
        await _uninstallPlugin
            .Received(1)
            .Uninstall(instanceInfo, "pluginId", "1.2.3", Arg.Any<CancellationToken>());
        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details =>
                    details.Id == projectId
                    && details.InstalledPlugins.SequenceEqual(
                        updatedProjectDetails.InstalledPlugins
                    )
                ),
                Arg.Any<CancellationToken>()
            );
    }
}

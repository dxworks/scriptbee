using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Plugins;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Plugins;

namespace ScriptBee.Service.Project.Tests.Plugins;

public class InstallPluginServiceTests
{
    private readonly IInstallPlugin _installPlugin = Substitute.For<IInstallPlugin>();
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly InstallPluginService _installPluginService;

    public InstallPluginServiceTests()
    {
        _installPluginService = new InstallPluginService(
            _installPlugin,
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

        var result = await _installPluginService.InstallPluginAsync(
            new InstallPluginCommand(projectId, "pluginId", "1.2.3"),
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenPluginVersionAlreadyInstalled_ExpectNoInteractions()
    {
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId) with
        {
            InstalledPlugins = [new PluginInstallationConfig("pluginId", "1.2.3")],
        };
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        var result = await _installPluginService.InstallPluginAsync(
            new InstallPluginCommand(projectId, "pluginId", "1.2.3"),
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(projectDetails);
        await _installPlugin
            .Received(0)
            .Install(
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
    public async Task GivenInstances_ExpectPluginInstalledToAllInstances()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var updatedProjectDetails = projectDetails with
        {
            InstalledPlugins = [new PluginInstallationConfig("pluginId", "1.2.3")],
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
        var result = await _installPluginService.InstallPluginAsync(
            new InstallPluginCommand(projectId, "pluginId", "1.2.3"),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.InstalledPlugins.ShouldBe(updatedProjectDetails.InstalledPlugins);
        await _installPlugin
            .Received(1)
            .Install(instanceInfo, "pluginId", "1.2.3", Arg.Any<CancellationToken>());
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

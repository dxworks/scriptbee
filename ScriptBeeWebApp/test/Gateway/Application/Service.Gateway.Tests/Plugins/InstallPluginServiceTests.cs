using Microsoft.Extensions.Logging;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins.Installer;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Gateway.Plugins;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Tests.Plugins;

public class InstallPluginServiceTests
{
    private readonly IBundlePluginInstaller _bundlePluginInstaller =
        Substitute.For<IBundlePluginInstaller>();

    private readonly IInstallPlugin _installPlugin = Substitute.For<IInstallPlugin>();
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly ILogger<InstallPluginService> _logger = Substitute.For<
        ILogger<InstallPluginService>
    >();

    private readonly InstallPluginService _installPluginService;

    public InstallPluginServiceTests()
    {
        _installPluginService = new InstallPluginService(
            _bundlePluginInstaller,
            _installPlugin,
            _getProject,
            _getAllProjectInstances,
            _updateProject,
            _logger
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
            new InstallPluginCommand(projectId, new PluginId("pluginId", new Version("1.2.3"))),
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
            InstalledPlugins = [new PluginInstallationConfig("pluginId", new Version("1.2.3"))],
        };
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        var result = await _installPluginService.InstallPluginAsync(
            new InstallPluginCommand(projectId, new PluginId("pluginId", new Version("1.2.3"))),
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(projectDetails);
        await _installPlugin
            .Received(0)
            .Install(Arg.Any<InstanceInfo>(), Arg.Any<PluginId>(), Arg.Any<CancellationToken>());
        await _updateProject
            .Received(0)
            .Update(Arg.Any<ProjectDetails>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInstances_ExpectPluginInstalledToAllInstances()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var pluginId = new PluginId("pluginId", new Version("1.2.3"));
        var nestedPluginId = new PluginId("nested-pluginId", new Version("1.0.0"));
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var updatedProjectDetails = projectDetails with
        {
            InstalledPlugins =
            [
                new PluginInstallationConfig("pluginId", new Version("1.2.3")),
                new PluginInstallationConfig("nested-pluginId", new Version("1.0.0")),
            ],
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
        _bundlePluginInstaller
            .Install(pluginId, Arg.Any<CancellationToken>())
            .Returns(new List<PluginId> { pluginId, nestedPluginId });
        _updateProject
            .Update(Arg.Any<ProjectDetails>(), Arg.Any<CancellationToken>())
            .Returns(updatedProjectDetails);

        // Act
        var result = await _installPluginService.InstallPluginAsync(
            new InstallPluginCommand(projectId, pluginId),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.InstalledPlugins.ShouldBe(updatedProjectDetails.InstalledPlugins);
        await _installPlugin
            .Received(1)
            .Install(instanceInfo, pluginId, Arg.Any<CancellationToken>());
        await _installPlugin
            .Received(1)
            .Install(instanceInfo, nestedPluginId, Arg.Any<CancellationToken>());
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

    #region Zip Stream Tests

    [Fact]
    public async Task GivenZipStream_WhenProjectDoesNotExist_ThenReturnsProjectDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        using var stream = new MemoryStream([1, 2, 3]);

        var result = await _installPluginService.InstallPluginAsync(
            projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenZipStream_WhenInstallReturnsManifestNotFound_ThenReturnsManifestNotFoundError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);

        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        using var stream = new MemoryStream([1, 2, 3]);

        _bundlePluginInstaller
            .Install(projectId, stream, Arg.Any<CancellationToken>())
            .Returns(new PluginManifestNotFoundError());

        var result = await _installPluginService.InstallPluginAsync(
            projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenZipStream_WhenInstallReturnsPluginAlreadyExists_ThenReturnsAlreadyExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var pluginId = new PluginId("existing", new Version("1.0.0"));
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);

        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        using var stream = new MemoryStream([1, 2, 3]);

        _bundlePluginInstaller
            .Install(projectId, stream, Arg.Any<CancellationToken>())
            .Returns(new PluginAlreadyExistsError(pluginId));

        var result = await _installPluginService.InstallPluginAsync(
            projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT3.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenZipStream_WhenInstallFails_ThenReturnsInstallationError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var pluginId = new PluginId("failedPlugin", new Version("1.0.0"));
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);

        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        using var stream = new MemoryStream([1, 2, 3]);

        _bundlePluginInstaller
            .Install(projectId, stream, Arg.Any<CancellationToken>())
            .Returns(new PluginInstallationError(pluginId, []));

        var result = await _installPluginService.InstallPluginAsync(
            projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT4.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenZipStream_WhenInstallSucceeds_ThenReturnsUpdatedProjectDetailsAndInstallsToInstances()
    {
        var projectId = ProjectId.FromValue("project-id");
        var pluginId = new PluginId("uploadedPlugin", new Version("2.0.0"));
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var updatedProjectDetails = projectDetails with
        {
            InstalledPlugins = [new PluginInstallationConfig(pluginId.Name, pluginId.Version)],
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

        using var stream = new MemoryStream([1, 2, 3]);

        _bundlePluginInstaller
            .Install(projectId, stream, Arg.Any<CancellationToken>())
            .Returns(new List<PluginId> { pluginId });

        _updateProject
            .Update(Arg.Any<ProjectDetails>(), Arg.Any<CancellationToken>())
            .Returns(updatedProjectDetails);

        var result = await _installPluginService.InstallPluginAsync(
            projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.InstalledPlugins.ShouldBe(updatedProjectDetails.InstalledPlugins, true);
        await _installPlugin
            .Received(1)
            .Install(instanceInfo, pluginId, Arg.Any<CancellationToken>());
    }

    #endregion
}

using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.Tests.Common;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class InstallPluginsForNewlyAllocatedInstanceTest
{
    private readonly IGetInstanceStatus _getInstanceStatus = Substitute.For<IGetInstanceStatus>();
    private readonly IInstallPlugin _installPlugin = Substitute.For<IInstallPlugin>();

    private readonly ILogger<InstallPluginsForNewlyAllocatedInstance> _logger = Substitute.For<
        ILogger<InstallPluginsForNewlyAllocatedInstance>
    >();

    private readonly InstallPluginsForNewlyAllocatedInstance _installPluginsForNewlyAllocatedInstance;

    public InstallPluginsForNewlyAllocatedInstanceTest()
    {
        _installPluginsForNewlyAllocatedInstance = new InstallPluginsForNewlyAllocatedInstance(
            _getInstanceStatus,
            _installPlugin,
            _logger
        );
    }

    [Fact]
    public async Task GivenInstanceRunning_ShouldInstallProjectPlugins()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId) with
        {
            InstalledPlugins = [new PluginInstallationConfig("plugin-id", "version")],
        };
        var instanceInfo = InstanceInfoFixture.BasicInstanceInfo(projectId);

        _getInstanceStatus
            .GetStatus(instanceInfo.Id, Arg.Any<CancellationToken>())
            .Returns(AnalysisInstanceStatus.Allocating, AnalysisInstanceStatus.Running);

        // Act
        await _installPluginsForNewlyAllocatedInstance.InstallPlugins(
            projectDetails,
            instanceInfo,
            TestContext.Current.CancellationToken
        );

        // Assert
        await _installPlugin
            .Received(1)
            .Install(instanceInfo, "plugin-id", "version", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInstanceNotFound_ShouldNotInstallAnything()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId) with
        {
            InstalledPlugins = [new PluginInstallationConfig("plugin-id", "version")],
        };
        var instanceInfo = InstanceInfoFixture.BasicInstanceInfo(projectId);

        _getInstanceStatus
            .GetStatus(instanceInfo.Id, Arg.Any<CancellationToken>())
            .Returns(AnalysisInstanceStatus.Allocating, AnalysisInstanceStatus.NotFound);

        // Act
        await _installPluginsForNewlyAllocatedInstance.InstallPlugins(
            projectDetails,
            instanceInfo,
            TestContext.Current.CancellationToken
        );

        // Assert
        await _installPlugin
            .Received(0)
            .Install(
                Arg.Any<InstanceInfo>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}

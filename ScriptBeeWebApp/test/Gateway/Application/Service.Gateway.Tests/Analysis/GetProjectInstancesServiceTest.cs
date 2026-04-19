using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.Service.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Tests.Analysis;

public class GetProjectInstancesServiceTest
{
    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly IGetInstanceStatus _getInstanceStatus = Substitute.For<IGetInstanceStatus>();

    private readonly GetProjectInstancesService _getProjectInstancesService;

    public GetProjectInstancesServiceTest()
    {
        _getProjectInstancesService = new GetProjectInstancesService(
            _getAllProjectInstances,
            _getProjectInstance,
            _getInstanceStatus
        );
    }

    [Fact]
    public async Task GetAllProjectInstances()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceInfo = new InstanceInfo(
            instanceId,
            projectId,
            "http://url:8080",
            DateTimeOffset.UtcNow,
            AnalysisInstanceStatus.Allocating
        );
        IEnumerable<InstanceInfo> expectedInstanceInfos = [instanceInfo];
        _getAllProjectInstances
            .GetAll(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedInstanceInfos));
        _getInstanceStatus
            .GetStatus(instanceId, Arg.Any<CancellationToken>())
            .Returns(AnalysisInstanceStatus.Allocating);

        var instanceInfos = await _getProjectInstancesService.GetAllInstances(
            projectId,
            TestContext.Current.CancellationToken
        );

        instanceInfos.Single().ShouldBe(instanceInfo);
    }

    [Fact]
    public async Task GivenNoInstance_WhenGetInstance_ShouldReturnInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var result = await _getProjectInstancesService.GetInstance(
            projectId,
            instanceId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new InstanceDoesNotExistsError(instanceId));
        await _getInstanceStatus
            .Received(0)
            .GetStatus(Arg.Any<InstanceId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInstance_WhenGetInstance_ShouldReturnInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceInfo = new InstanceInfo(
            instanceId,
            projectId,
            "http://url:8080",
            DateTimeOffset.UtcNow,
            AnalysisInstanceStatus.Allocating
        );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );
        _getInstanceStatus
            .GetStatus(instanceId, Arg.Any<CancellationToken>())
            .Returns(AnalysisInstanceStatus.Running);

        var result = await _getProjectInstancesService.GetInstance(
            projectId,
            instanceId,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(instanceInfo with { Status = AnalysisInstanceStatus.Running });
    }
}

using NSubstitute;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.Service.Project.Analysis;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class GetProjectInstancesServiceTest
{
    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IGetInstanceStatus _getInstanceStatus = Substitute.For<IGetInstanceStatus>();

    private readonly GetProjectInstancesService _getProjectInstancesService;

    public GetProjectInstancesServiceTest()
    {
        _getProjectInstancesService = new GetProjectInstancesService(
            _getAllProjectInstances,
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
}

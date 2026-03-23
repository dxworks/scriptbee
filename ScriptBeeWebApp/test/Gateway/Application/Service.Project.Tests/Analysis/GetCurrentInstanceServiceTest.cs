using NSubstitute;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.Tests.Common;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class GetCurrentInstanceServiceTest
{
    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly GetCurrentInstanceService _getCurrentInstanceService;

    public GetCurrentInstanceServiceTest()
    {
        _getCurrentInstanceService = new GetCurrentInstanceService(_getAllProjectInstances);
    }

    [Fact]
    public async Task GivenNoInstanceForProjectId_ThenExpectNoInstanceAllocatedForProjectError()
    {
        var projectId = ProjectId.FromValue("id");
        _getAllProjectInstances
            .GetAll(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<InstanceInfo>>(new List<InstanceInfo>()));

        var result = await _getCurrentInstanceService.GetCurrentInstance(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new NoInstanceAllocatedForProjectError(projectId));
    }

    [Fact]
    public async Task GivenInstancesForProjectId_ThenReturnFirstInstance()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceInfo1 = InstanceInfoFixture.BasicInstanceInfo(projectId) with
        {
            Id = new InstanceId("5ded901e-f237-449e-9782-5a3b3b1d7b6f"),
        };
        var instanceInfo2 = InstanceInfoFixture.BasicInstanceInfo(projectId);
        _getAllProjectInstances
            .GetAll(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<InstanceInfo>>(
                    new List<InstanceInfo> { instanceInfo1, instanceInfo2 }
                )
            );

        var result = await _getCurrentInstanceService.GetCurrentInstance(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(instanceInfo1);
    }
}

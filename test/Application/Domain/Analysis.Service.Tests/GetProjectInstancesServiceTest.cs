using NSubstitute;
using ScriptBee.Analysis.Ports;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Analysis.Service.Tests;

public class GetProjectInstancesServiceTest
{
    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly GetProjectInstancesService _getProjectInstancesService;

    public GetProjectInstancesServiceTest()
    {
        _getProjectInstancesService = new GetProjectInstancesService(_getAllProjectInstances);
    }

    [Fact]
    public async Task GetAllProjectInstances()
    {
        var projectId = ProjectId.FromValue("project-id");
        IEnumerable<InstanceInfo> expectedCalculationInstanceInfos =
        [
            new(
                InstanceId.FromValue("id"),
                projectId,
                "http://url:8080",
                DateTimeOffset.UtcNow
            ),
        ];
        _getAllProjectInstances
            .GetAll(projectId)
            .Returns(Task.FromResult(expectedCalculationInstanceInfos));

        var instanceInfos = await _getProjectInstancesService.GetAllInstances(projectId);

        instanceInfos.ShouldBeEquivalentTo(expectedCalculationInstanceInfos);
    }
}

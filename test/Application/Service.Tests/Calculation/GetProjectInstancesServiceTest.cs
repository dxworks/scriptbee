using NSubstitute;
using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Calculation;
using ScriptBee.Ports.Driven.Calculation;
using Shouldly;

namespace ScriptBee.Domain.Service.Tests.Calculation;

public class GetProjectInstancesServiceTest
{
    private readonly IGetAllProjectInstances _getAllProjectInstances = Substitute.For<IGetAllProjectInstances>();
    private readonly GetProjectInstancesService _getProjectInstancesService;

    public GetProjectInstancesServiceTest()
    {
        _getProjectInstancesService = new GetProjectInstancesService(_getAllProjectInstances);
    }

    [Fact]
    public async Task GetAllProjectInstances()
    {
        var projectId = ProjectId.FromValue("project-id");
        IEnumerable<CalculationInstanceInfo> expectedCalculationInstanceInfos =
            [new(CalculationInstanceId.FromValue("id"), projectId, "http://url:8080", DateTimeOffset.UtcNow)];
        _getAllProjectInstances.GetAll(projectId)
            .Returns(Task.FromResult(expectedCalculationInstanceInfos));

        var instanceInfos = await _getProjectInstancesService.GetAllInstances(projectId);

        instanceInfos.ShouldBeEquivalentTo(expectedCalculationInstanceInfos);
    }
}

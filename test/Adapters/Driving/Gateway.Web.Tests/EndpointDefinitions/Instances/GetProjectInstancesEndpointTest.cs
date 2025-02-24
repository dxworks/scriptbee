using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Web.EndpointDefinitions.Instances.Contracts;
using ScriptBee.Ports.Driving.UseCases.Calculation;
using ScriptBee.Tests.Common;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Instances;

public class GetProjectInstancesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/instances";
    private readonly TestApiCaller _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnProjectDetailsList()
    {
        var getProjectInstances = Substitute.For<IGetProjectInstancesUseCase>();
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        var projectId = ProjectId.Create("project-id");
        IEnumerable<CalculationInstanceInfo> projectDetailsList = new List<CalculationInstanceInfo>
        {
            new(
                CalculationInstanceId.FromValue("instance-id"),
                projectId,
                "http://url",
                creationDate
            ),
        };
        getProjectInstances
            .GetAllInstances(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(projectDetailsList));

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(getProjectInstances);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getProjectListResponse =
            await response.ReadContentAsync<WebGetProjectInstancesListResponse>();
        getProjectListResponse.Instances.ShouldBeEquivalentTo(
            new List<WebGetProjectInstance> { new("instance-id", creationDate) }
        );
    }
}

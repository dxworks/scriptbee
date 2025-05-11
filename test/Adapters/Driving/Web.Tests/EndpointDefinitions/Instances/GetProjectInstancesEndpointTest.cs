using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Instances.Contracts;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Instances;

public class GetProjectInstancesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/instances";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnProjectDetailsList()
    {
        var useCase = Substitute.For<IGetProjectInstancesUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        var instanceInfo = InstanceInfoFixture.BasicInstanceInfo(projectId);
        IEnumerable<InstanceInfo> projectDetailsList = new List<InstanceInfo> { instanceInfo };
        useCase
            .GetAllInstances(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(projectDetailsList));

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getProjectListResponse =
            await response.ReadContentAsync<WebGetProjectInstancesListResponse>();
        getProjectListResponse.Instances.ShouldBeEquivalentTo(
            new List<WebProjectInstance>
            {
                new(instanceInfo.Id.ToString(), instanceInfo.CreationDate),
            }
        );
    }
}

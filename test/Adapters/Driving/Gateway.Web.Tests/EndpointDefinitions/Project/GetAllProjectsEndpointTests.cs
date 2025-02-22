using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Ports.Driving.UseCases.Project;
using ScriptBee.Tests.Common;
using Shouldly;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Project;

public class GetAllProjectsEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects";
    private readonly TestApiCaller _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnProjectDetailsList()
    {
        var getProjectsUseCase = Substitute.For<IGetProjectsUseCase>();
        var creationDate = DateTime.Parse("2024-02-08");
        getProjectsUseCase.GetAllProjects(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<ProjectDetails>
            {
                new(ProjectId.Create("id"), "name", creationDate)
            }));

        var response = await _api.GetApi(new TestWebApplicationFactory<Program>(outputHelper,
            services => { services.AddSingleton(getProjectsUseCase); }));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getProjectListResponse = await response.ReadContentAsync<WebGetProjectListResponse>();
        getProjectListResponse.ShouldBeEquivalentTo(
            new WebGetProjectListResponse([new WebGetProjectDetailsResponse("id", "name", creationDate)])
        );
    }
}

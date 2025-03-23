using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Project;

public class GetProjectByIdEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnProjectDetails()
    {
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var getProjectsUseCase = Substitute.For<IGetProjectsUseCase>();
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        getProjectsUseCase
            .GetProject(query, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDetails(
                        projectId,
                        "name",
                        creationDate,
                        new Dictionary<string, List<FileData>>()
                    )
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(getProjectsUseCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getResponse = await response.ReadContentAsync<WebGetProjectDetailsResponse>();
        getResponse.ShouldBe(new WebGetProjectDetailsResponse("id", "name", creationDate));
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var getProjectsUseCase = Substitute.For<IGetProjectsUseCase>();
        getProjectsUseCase
            .GetProject(query, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(getProjectsUseCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Project Not Found",
            "A project with the ID 'id' does not exists."
        );
    }
}

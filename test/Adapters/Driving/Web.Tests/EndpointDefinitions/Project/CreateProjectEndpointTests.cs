using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Project;

public class CreateProjectEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task InvalidRequestBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebCreateProjectCommand("id", "")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { Name = new List<string> { "'Name' must not be empty." } }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi<WebCreateProjectCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ShouldReturnCreated()
    {
        var createProjectUseCase = Substitute.For<ICreateProjectUseCase>();
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        createProjectUseCase
            .CreateProject(
                new CreateProjectCommand("id", "project name"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
                    new ProjectDetails(ProjectId.Create("id"), "name", creationDate)
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(createProjectUseCase);
                }
            ),
            new WebCreateProjectCommand("id", "project name")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var createProjectResponse = await response.ReadContentAsync<WebCreateProjectResponse>();
        createProjectResponse.Id.ShouldBe("id");
        createProjectResponse.Name.ShouldBe("name");
        createProjectResponse.CreationDate.ShouldBe(creationDate);
    }

    [Fact]
    public async Task ExistingId_ShouldReturnConflict()
    {
        var createProjectUseCase = Substitute.For<ICreateProjectUseCase>();
        createProjectUseCase
            .CreateProject(
                new CreateProjectCommand("id", "project name"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
                    new ProjectIdAlreadyInUseError(ProjectId.Create("id"))
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(createProjectUseCase);
                }
            ),
            new WebCreateProjectCommand("id", "project name")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await AssertConflictProblem(
            response.Content,
            TestUrl,
            "Project ID Already In Use",
            "A project with the ID 'id' already exists. Use a unique Project ID or update the existing project."
        );
    }
}

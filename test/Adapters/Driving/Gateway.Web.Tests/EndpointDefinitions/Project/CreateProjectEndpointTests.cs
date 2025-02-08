using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Authorization;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Ports.Driving.UseCases.Project;
using ScriptBee.Tests.Common;
using Shouldly;
using Xunit.Abstractions;
using static ScriptBee.Gateway.Web.Tests.ProblemValidationUtils;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Project;

public class CreateProjectEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/scriptbee/projects";
    private readonly TestApiCaller _api = new(TestUrl);

    [Fact]
    public async Task AdministratorRoleWithInvalidRequestBody_ShouldReturnBadRequest()
    {
        var response =
            await _api.PostApi(new TestWebApplicationFactory<Program>(outputHelper, [UserRole.Administrator]),
                new WebCreateProjectCommand("id", ""));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(response.Content, TestUrl,
            new
            {
                Name = new List<string> { "'Name' must not be empty." }
            });
    }

    [Fact]
    public async Task AdministratorRoleWithEmptyBody_ShouldReturnBadRequest()
    {
        var response =
            await _api.PostApi<WebCreateProjectCommand>(
                new TestWebApplicationFactory<Program>(outputHelper, [UserRole.Administrator]));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task AdministratorRole_ShouldReturnCreated()
    {
        var createProjectUseCase = Substitute.For<ICreateProjectUseCase>();
        createProjectUseCase.CreateProject(new CreateProjectCommand("id", "project name"), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
                new ProjectDetails(ProjectId.Create("id"), "name", DateTime.Parse("2024-02-08"))));

        var response =
            await _api.PostApi(new TestWebApplicationFactory<Program>(outputHelper, [UserRole.Administrator],
                    services => { services.AddSingleton(createProjectUseCase); }),
                new WebCreateProjectCommand("id", "project name"));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var createProjectResponse = await response.ReadContentAsync<WebCreateProjectResponse>();
        createProjectResponse.Id.ShouldBe("id");
        createProjectResponse.Name.ShouldBe("name");
    }

    [Fact]
    public async Task AdministratorRoleAndExistingId_ShouldReturnConflict()
    {
        var createProjectUseCase = Substitute.For<ICreateProjectUseCase>();
        createProjectUseCase.CreateProject(new CreateProjectCommand("id", "project name"), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
                new ProjectIdAlreadyInUseError(ProjectId.Create("id"))));

        var response =
            await _api.PostApi(new TestWebApplicationFactory<Program>(outputHelper, [UserRole.Administrator],
                    services => { services.AddSingleton(createProjectUseCase); }),
                new WebCreateProjectCommand("id", "project name"));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await AssertConflictProblem(response.Content, TestUrl,
            "Project ID Already In Use",
            "A project with the ID 'id' already exists. Use a unique Project ID or update the existing project."
        );
    }

    [Fact]
    public async Task OtherRole_ShouldReturnForbidden()
    {
        var response =
            await _api.PostApi<WebCreateProjectCommand>(
                new TestWebApplicationFactory<Program>(outputHelper, [UserRole.Guest]));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task NoRoles_ShouldReturnForbidden()
    {
        var response =
            await _api.PostApi<WebCreateProjectCommand>(new TestWebApplicationFactory<Program>(outputHelper, []));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UnauthorizedUser_ShouldReturnUnauthorized()
    {
        var response = await _api.PostApiWithoutAuthorization(new TestWebApplicationFactory<Program>(outputHelper));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

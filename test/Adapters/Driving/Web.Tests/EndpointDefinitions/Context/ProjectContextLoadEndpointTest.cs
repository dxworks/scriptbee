using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class ProjectContextLoadEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/60db5e7e-38ec-4fc3-b810-71eebbc206bd/context/load";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task InvalidRequestBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebLoadContextCommand(null!)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { LoaderIds = new List<string> { "'Loader Ids' must not be empty." } }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi<WebLoadContextCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ContexLoadSuccessful_ShouldReturnNoContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("60db5e7e-38ec-4fc3-b810-71eebbc206bd");
        var useCase = Substitute.For<ILoadInstanceContextUseCase>();
        var expectedCommand = new LoadContextCommand(projectId, instanceId, ["loader-id"]);
        useCase
            .Load(
                Arg.Is<LoadContextCommand>(actual =>
                    LoadContextCommandMatcher(actual, expectedCommand)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>>(
                    new Unit()
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLoadContextCommand(["loader-id"])
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<ILoadInstanceContextUseCase>();
        useCase
            .Load(Arg.Any<LoadContextCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(ProjectId.FromValue("project-id"))
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLoadContextCommand(["loader-id"])
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Project Not Found",
            $"A project with the ID 'project-id' does not exists."
        );
    }

    [Fact]
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        var instanceId = new InstanceId("60db5e7e-38ec-4fc3-b810-71eebbc206bd");
        var useCase = Substitute.For<ILoadInstanceContextUseCase>();
        useCase
            .Load(Arg.Any<LoadContextCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLoadContextCommand(["loader-id"])
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Instance Not Found",
            "An instance with id '60db5e7e-38ec-4fc3-b810-71eebbc206bd' is not allocated."
        );
    }

    private static bool LoadContextCommandMatcher(
        LoadContextCommand actual,
        LoadContextCommand expected
    )
    {
        return actual.ProjectId.Equals(expected.ProjectId)
            && actual.InstanceId.Equals(expected.InstanceId)
            && actual.LoaderIds.SequenceEqual(expected.LoaderIds);
    }
}

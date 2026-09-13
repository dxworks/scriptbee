using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
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
        // Act
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebLoadContextCommand(null, null)
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new
            {
                LoaderIds = new List<string>
                {
                    "Either 'LoaderIds' or 'FilesToLoad' must be provided and non-empty.",
                },
            }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        // Act
        var response = await _api.PostApi<WebLoadContextCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ContextLoadSuccessful_ShouldReturnNoContent()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("60db5e7e-38ec-4fc3-b810-71eebbc206bd");
        var filesToLoad = new Dictionary<string, List<string>> { { "loader-id", ["file-id"] } };
        var useCase = Substitute.For<ILoadInstanceContextUseCase>();
        var expectedCommand = new LoadContextCommand(projectId, instanceId, null, filesToLoad);
        useCase
            .Load(
                Arg.Is<LoadContextCommand>(actual =>
                    LoadContextCommandMatcher(actual, expectedCommand)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>
                >(new Success())
            );

        // Act
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLoadContextCommand(null, filesToLoad)
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var filesToLoad = new Dictionary<string, List<string>> { { "loader-id", ["file-id"] } };
        var useCase = Substitute.For<ILoadInstanceContextUseCase>();
        useCase
            .Load(Arg.Any<LoadContextCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>
                >(new ProjectDoesNotExistsError(ProjectId.FromValue("project-id")))
            );

        // Act
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLoadContextCommand(null, filesToLoad)
        );

        // Assert
        await AssertProjectNotFoundProblem(response, TestUrl);
    }

    [Fact]
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var filesToLoad = new Dictionary<string, List<string>> { { "loader-id", ["file-id"] } };
        var instanceId = new InstanceId("60db5e7e-38ec-4fc3-b810-71eebbc206bd");
        var useCase = Substitute.For<ILoadInstanceContextUseCase>();
        useCase
            .Load(Arg.Any<LoadContextCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>
                >(new InstanceDoesNotExistsError(instanceId))
            );

        // Act
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLoadContextCommand(null, filesToLoad)
        );

        // Assert
        await AssertInstanceNotFoundProblem(
            response,
            TestUrl,
            "60db5e7e-38ec-4fc3-b810-71eebbc206bd"
        );
    }

    private static bool LoadContextCommandMatcher(
        LoadContextCommand actual,
        LoadContextCommand expected
    )
    {
        return actual.ProjectId.Equals(expected.ProjectId)
            && actual.InstanceId.Equals(expected.InstanceId)
            && actual.FilesToLoad?.Count == expected.FilesToLoad?.Count
            && (actual.FilesToLoad?.Keys ?? []).All(k =>
                (expected.FilesToLoad?.ContainsKey(k) ?? false)
                && (actual.FilesToLoad?[k].SequenceEqual(expected.FilesToLoad?[k] ?? []) ?? false)
            );
    }
}

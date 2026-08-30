using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectAccess;

public class ManageProjectTokensEndpointTests(ITestOutputHelper outputHelper)
{
    private readonly TestApiCaller<Program> _api = new("/api/projects/project-id/tokens");

    [Theory]
    [FilePath("TestData/GetProjectTokens/response.json")]
    public async Task GetProjectTokens_ShouldReturnTokens(string responsePath)
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IManageProjectTokensUseCase>();
        var createdAt = DateTimeOffset.Parse("2024-02-08T00:00:00+00:00");
        var expiresAt = DateTimeOffset.Parse("2024-03-10T00:00:00+00:00");

        useCase
            .GetProjectTokens(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new List<ProjectToken>
                    {
                        new(
                            new ProjectTokenId("token-1"),
                            projectId,
                            "hash-1",
                            "CI token",
                            new UserRole("viewer"),
                            createdAt,
                            expiresAt
                        ),
                        new(
                            new ProjectTokenId("token-2"),
                            projectId,
                            "hash-2",
                            "Deploy token",
                            new UserRole("owner"),
                            createdAt.AddDays(1),
                            expiresAt.AddDays(1)
                        ),
                    }
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetProjectTokens/empty_response.json")]
    public async Task GetProjectTokens_WhenNoTokens_ShouldReturnEmptyList(string responsePath)
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IManageProjectTokensUseCase>();

        useCase
            .GetProjectTokens(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<ProjectToken>()));

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/CreateProjectToken/response.json")]
    public async Task CreateProjectToken_ShouldReturnToken(string responsePath)
    {
        var useCase = Substitute.For<IManageProjectTokensUseCase>();
        var createdAt = DateTimeOffset.Parse("2024-02-08T00:00:00+00:00");
        var expiresAt = DateTimeOffset.Parse("2024-03-10T00:00:00+00:00");
        var projectId = ProjectId.FromValue("project-id");

        useCase
            .CreateProjectToken(Arg.Any<CreateProjectTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new NewProjectTokenResult(
                        new ProjectToken(
                            new ProjectTokenId("token-1"),
                            projectId,
                            "hash-1",
                            "CI token",
                            new UserRole("viewer"),
                            createdAt,
                            expiresAt
                        ),
                        "plain-token"
                    )
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new WebCreateProjectTokenRequest("CI token", "viewer", expiresAt)
        );

        await response.AssertResponse(HttpStatusCode.Created, responsePath);
        await useCase
            .Received(1)
            .CreateProjectToken(
                new CreateProjectTokenCommand(
                    projectId,
                    "CI token",
                    new UserRole("viewer"),
                    expiresAt
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task CreateProjectToken_InvalidBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebCreateProjectTokenRequest("CI token", "", DateTimeOffset.UtcNow)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveProjectToken_ShouldReturnNoContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IManageProjectTokensUseCase>();

        useCase
            .DeleteProjectToken(
                projectId,
                new ProjectTokenId("token-1"),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.CompletedTask);

        var response = await new TestApiCaller<Program>(
            "/api/projects/project-id/tokens/token-1"
        ).DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase
            .Received(1)
            .DeleteProjectToken(
                projectId,
                new ProjectTokenId("token-1"),
                Arg.Any<CancellationToken>()
            );
    }
}

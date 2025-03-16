using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class ProjectContextLinkEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/8be03260-c9e4-4597-94b3-c97ba047724e/context/link";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task InvalidRequestBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebLinkContextCommand(null!)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { LinkerIds = new List<string> { "'Linker Ids' must not be empty." } }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi<WebLinkContextCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ContexLinkSuccessful_ShouldReturnNoContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("8be03260-c9e4-4597-94b3-c97ba047724e");
        var useCase = Substitute.For<ILinkInstanceContextUseCase>();
        var expectedCommand = new LinkContextCommand(projectId, instanceId, ["linker-id"]);
        useCase
            .Link(
                Arg.Is<LinkContextCommand>(actual =>
                    LinkContextCommandMatcher(actual, expectedCommand)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<OneOf<Unit, InstanceDoesNotExistsError>>(new Unit()));

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLinkContextCommand(["linker-id"])
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        var instanceId = new InstanceId("8be03260-c9e4-4597-94b3-c97ba047724e");
        var useCase = Substitute.For<ILinkInstanceContextUseCase>();
        useCase
            .Link(Arg.Any<LinkContextCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Unit, InstanceDoesNotExistsError>>(
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
            new WebLinkContextCommand(["linker-id"])
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Instance Not Found",
            "An instance with id '8be03260-c9e4-4597-94b3-c97ba047724e' is not allocated."
        );
    }

    private static bool LinkContextCommandMatcher(
        LinkContextCommand actual,
        LinkContextCommand expected
    )
    {
        return actual.ProjectId.Equals(expected.ProjectId)
            && actual.InstanceId.Equals(expected.InstanceId)
            && actual.LinkerIds.SequenceEqual(expected.LinkerIds);
    }
}

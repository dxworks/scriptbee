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
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class ProjectContextReloadEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/ba16d778-4e65-46d3-ac49-b851f5d01434/context/reload";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ContexReloadSuccessful_ShouldReturnNoContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("ba16d778-4e65-46d3-ac49-b851f5d01434");
        var useCase = Substitute.For<IReloadInstanceContextUseCase>();
        useCase
            .Reload(new ReloadContextCommand(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>>(
                    new Unit()
                )
            );

        var response = await _api.PostApi<object>(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("ba16d778-4e65-46d3-ac49-b851f5d01434");
        var useCase = Substitute.For<IReloadInstanceContextUseCase>();
        useCase
            .Reload(new ReloadContextCommand(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var response = await _api.PostApi<object>(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Project Not Found",
            "A project with the ID 'project-id' does not exists."
        );
    }

    [Fact]
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("ba16d778-4e65-46d3-ac49-b851f5d01434");
        var useCase = Substitute.For<IReloadInstanceContextUseCase>();
        useCase
            .Reload(new ReloadContextCommand(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var response = await _api.PostApi<object>(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Instance Not Found",
            "An instance with id 'ba16d778-4e65-46d3-ac49-b851f5d01434' is not allocated."
        );
    }
}

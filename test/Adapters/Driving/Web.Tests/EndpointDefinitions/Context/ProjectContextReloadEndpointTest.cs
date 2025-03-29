using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
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
        "/api/projects/project-id/instances/b6a9a670-481a-4c0c-a563-fece9008c3c6/context/reload";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ContexClearSuccessful_ShouldReturnNoContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("b6a9a670-481a-4c0c-a563-fece9008c3c6");
        var useCase = Substitute.For<IClearInstanceContextUseCase>();
        useCase
            .Clear(new ClearContextCommand(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Unit, InstanceDoesNotExistsError>>(new Unit()));

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
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("b6a9a670-481a-4c0c-a563-fece9008c3c6");
        var useCase = Substitute.For<IClearInstanceContextUseCase>();
        useCase
            .Clear(new ClearContextCommand(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Unit, InstanceDoesNotExistsError>>(
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
            "An instance with id 'b6a9a670-481a-4c0c-a563-fece9008c3c6' is not allocated."
        );
    }
}

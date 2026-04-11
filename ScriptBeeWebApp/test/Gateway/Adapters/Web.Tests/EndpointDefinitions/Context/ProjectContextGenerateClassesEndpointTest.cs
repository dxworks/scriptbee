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
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class ProjectContextGenerateClassesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/edd5a455-cc96-4701-9d7c-118c052aa965/context/generate-classes";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task GenerateClassesSuccessful_ShouldReturnNoContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("edd5a455-cc96-4701-9d7c-118c052aa965");
        var useCase = Substitute.For<IGenerateInstanceClassesUseCase>();
        useCase
            .Generate(
                new GenerateClassesCommand(projectId, instanceId),
                Arg.Any<CancellationToken>()
            )
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
        var instanceId = new InstanceId("edd5a455-cc96-4701-9d7c-118c052aa965");
        var useCase = Substitute.For<IGenerateInstanceClassesUseCase>();
        useCase
            .Generate(
                new GenerateClassesCommand(projectId, instanceId),
                Arg.Any<CancellationToken>()
            )
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

        await AssertInstanceNotFoundProblem(
            response,
            TestUrl,
            "edd5a455-cc96-4701-9d7c-118c052aa965"
        );
    }
}

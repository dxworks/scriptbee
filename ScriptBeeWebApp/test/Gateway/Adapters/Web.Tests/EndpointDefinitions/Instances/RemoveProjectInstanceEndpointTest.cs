using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Instances;

public class RemoveProjectInstanceEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/d273cd34-96a4-4daa-8fd0-dffd6e8f5a1e";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnNoContent()
    {
        var useCase = Substitute.For<IDeallocateProjectInstanceUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("d273cd34-96a4-4daa-8fd0-dffd6e8f5a1e");
        useCase
            .Deallocate(projectId, instanceId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Success, ProjectDoesNotExistsError>>(new Success()));

        var response = await _api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase.Received(1).Deallocate(projectId, instanceId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IDeallocateProjectInstanceUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("d273cd34-96a4-4daa-8fd0-dffd6e8f5a1e");
        useCase
            .Deallocate(projectId, instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Success, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var response = await _api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertProjectNotFoundProblem(response, TestUrl);
    }
}

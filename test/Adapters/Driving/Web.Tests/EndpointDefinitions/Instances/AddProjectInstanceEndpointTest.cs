using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.InstanceInfoFixture;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Instances;

public class AddProjectInstanceEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/instances";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnCreated()
    {
        var useCase = Substitute.For<IAllocateProjectInstanceUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        var instanceInfo = BasicInstanceInfo(projectId);
        useCase
            .Allocate(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<InstanceInfo, ProjectDoesNotExistsError>>(instanceInfo));

        var response = await _api.PostApi<object>(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response
            .Headers.Location?.ToString()
            .ShouldBe($"/api/projects/project-id/instances/{instanceInfo.Id}");
        var createProjectResponse = await response.ReadContentAsync<WebCreateProjectResponse>();
        createProjectResponse.Id.ShouldBe(instanceInfo.Id.ToString());
        createProjectResponse.CreationDate.ShouldBe(instanceInfo.CreationDate);
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IAllocateProjectInstanceUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        useCase
            .Allocate(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, ProjectDoesNotExistsError>>(
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
}

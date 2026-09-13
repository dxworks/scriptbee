using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Context;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class UnloadInstanceContextEndpointTest(ITestOutputHelper outputHelper)
{
    private const string FileUrl =
        "/api/projects/project-id/instances/01a0992a-060d-77cb-a113-190be04253c8/context/loaders/loader-id/files/2855aa70-c204-4361-8729-33f8df7dfd18";
    private const string LoaderUrl =
        "/api/projects/project-id/instances/01a0992a-060d-77cb-a113-190be04253c8/context/loaders/loader-id";

    [Fact]
    public async Task UnloadFile_Successful_ShouldReturnNoContent()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("01a0992a-060d-77cb-a113-190be04253c8");
        var fileId = new FileId("2855aa70-c204-4361-8729-33f8df7dfd18");
        var useCase = Substitute.For<IUnloadInstanceContextUseCase>();
        useCase
            .Unload(
                Arg.Is<UnloadInstanceContextCommand>(cmd =>
                    cmd.ProjectId.Equals(projectId)
                    && cmd.InstanceId.Equals(instanceId)
                    && cmd.LoaderId == "loader-id"
                    && cmd.FileId != null
                    && cmd.FileId.Equals(fileId)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>
                >(new Success())
            );

        // Act
        var api = new TestApiCaller<Program>(FileUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UnloadLoader_Successful_ShouldReturnNoContent()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("01a0992a-060d-77cb-a113-190be04253c8");
        var useCase = Substitute.For<IUnloadInstanceContextUseCase>();
        useCase
            .Unload(
                Arg.Is<UnloadInstanceContextCommand>(cmd =>
                    cmd.ProjectId.Equals(projectId)
                    && cmd.InstanceId.Equals(instanceId)
                    && cmd.LoaderId == "loader-id"
                    && cmd.FileId == null
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>
                >(new Success())
            );

        // Act
        var api = new TestApiCaller<Program>(LoaderUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var useCase = Substitute.For<IUnloadInstanceContextUseCase>();
        useCase
            .Unload(Arg.Any<UnloadInstanceContextCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>
                >(new ProjectDoesNotExistsError(ProjectId.FromValue("project-id")))
            );

        // Act
        var api = new TestApiCaller<Program>(FileUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        await AssertProjectNotFoundProblem(response, FileUrl);
    }

    [Fact]
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var instanceId = new InstanceId("01a0992a-060d-77cb-a113-190be04253c8");
        var useCase = Substitute.For<IUnloadInstanceContextUseCase>();
        useCase
            .Unload(Arg.Any<UnloadInstanceContextCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>
                >(new InstanceDoesNotExistsError(instanceId))
            );

        // Act
        var api = new TestApiCaller<Program>(FileUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        await AssertInstanceNotFoundProblem(
            response,
            FileUrl,
            "01a0992a-060d-77cb-a113-190be04253c8"
        );
    }
}

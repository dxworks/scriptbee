using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Files;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Files;

public class DeleteSavedFileEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/saved-files/cfbff0d1-9375-5685-968c-48ce8b15ae17";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task DeleteSuccessful_ShouldReturnNoContent()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var fileId = new FileId("cfbff0d1-9375-5685-968c-48ce8b15ae17");
        var useCase = Substitute.For<IDeleteSavedFileUseCase>();
        useCase
            .Delete(
                Arg.Is<DeleteSavedFileCommand>(cmd =>
                    cmd.ProjectId.Equals(projectId) && cmd.FileId.Equals(fileId)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, ProjectFileDoesNotExistsError>
                >(new Success())
            );

        // Act
        var response = await _api.DeleteApi(
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
        var useCase = Substitute.For<IDeleteSavedFileUseCase>();
        useCase
            .Delete(Arg.Any<DeleteSavedFileCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, ProjectFileDoesNotExistsError>
                >(new ProjectDoesNotExistsError(ProjectId.FromValue("project-id")))
            );

        // Act
        var response = await _api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        await AssertProjectNotFoundProblem(response, TestUrl);
    }

    [Fact]
    public async Task FileNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var useCase = Substitute.For<IDeleteSavedFileUseCase>();
        useCase
            .Delete(Arg.Any<DeleteSavedFileCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Success, ProjectDoesNotExistsError, ProjectFileDoesNotExistsError>
                >(
                    new ProjectFileDoesNotExistsError(
                        new FileId("cfbff0d1-9375-5685-968c-48ce8b15ae17")
                    )
                )
            );

        // Act
        var response = await _api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Files;
using ScriptBee.Web.EndpointDefinitions.Files.Contracts;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Files;

public class UploadSavedFilesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/saved-files";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        // Act
        var response = await _api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(outputHelper),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]>()
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ShouldReturnOk()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IUploadSavedFilesUseCase>();
        useCase
            .Upload(
                Arg.Is<UploadSavedFilesCommand>(command =>
                    command.ProjectId.Equals(projectId)
                    && command.UploadFiles.Select(f => f.FileName).Single().Equals("file-1")
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>>(
                    new List<FileData>
                    {
                        new(new FileId("b1e491f8-502f-4d80-96e8-6087d0cc7447"), "file-1"),
                    }
                )
            );

        // Act
        var response = await _api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]> { { "file-1", "test-content"u8.ToArray() } }
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.ReadContentAsync<WebUploadSavedFilesResponse>();
        var file = content.Files.Single();
        file.Id.ShouldBe("b1e491f8-502f-4d80-96e8-6087d0cc7447");
        file.Name.ShouldBe("file-1");
    }

    [Fact]
    public async Task GivenProjectDoesNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IUploadSavedFilesUseCase>();
        useCase
            .Upload(
                Arg.Is<UploadSavedFilesCommand>(command =>
                    command.ProjectId.Equals(projectId)
                    && command.UploadFiles.Select(f => f.FileName).Single().Equals("file-1")
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        // Act
        var response = await _api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]> { { "file-1", "test-content"u8.ToArray() } }
        );

        // Assert
        await AssertProjectNotFoundProblem(response, TestUrl);
    }
}

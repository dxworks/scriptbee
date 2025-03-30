using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Files;
using ScriptBee.Web.EndpointDefinitions.Loaders.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Loaders;

public class UploadLoaderFilesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/loaders/loader-id/files";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PutApiFormWithFile(
            new TestWebApplicationFactory<Program>(outputHelper),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]>()
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ShouldReturnOk()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IUploadLoaderFilesUseCase>();
        useCase
            .Upload(
                Arg.Is<UploadLoaderFilesCommand>(command =>
                    command.ProjectId.Equals(projectId)
                    && command.LoaderId.Equals("loader-id")
                    && command.UploadFiles.Select(f => f.FileName).Single().Equals("file-1")
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>>(
                    new List<FileData>
                    {
                        new(new FileId("b1e491f8-502f-4d80-96e8-6087d0cc7447"), "name"),
                    }
                )
            );

        var response = await _api.PutApiFormWithFile(
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

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.ReadContentAsync<WebUploadLoaderFilesResponse>();
        content.LoaderId.ShouldBe("loader-id");
        content.FileNames.ToList().ShouldBe(["name"]);
    }

    [Fact]
    public async Task GivenProjectDoesNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IUploadLoaderFilesUseCase>();
        useCase
            .Upload(
                Arg.Is<UploadLoaderFilesCommand>(command =>
                    command.ProjectId.Equals(projectId)
                    && command.LoaderId.Equals("loader-id")
                    && command.UploadFiles.Select(f => f.FileName).Single().Equals("file-1")
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var response = await _api.PutApiFormWithFile(
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

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertProjectNotFoundProblem(response, TestUrl);
    }
}

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project;
using static ScriptBee.Tests.Common.ProblemValidationUtils;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Project;

public class GetProjectByIdEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetProjectById/response.json")]
    public async Task ShouldReturnProjectDetails(string responsePath)
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var useCase = Substitute.For<IGetProjectsUseCase>();
        var creationDate = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        var projectDetails = BasicProjectDetails(ProjectId.Create("id"), "name", creationDate) with
        {
            SavedFiles = new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("f2461a1d-b63a-4f7f-a486-d6b1aad57ad9"), "file-name")]
                },
            },
            LoadedFiles = new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("f2461a1d-b63a-4f7f-a486-d6b1aad57ad9"), "file-name")]
                },
            },
            Linkers = ["linker-id"],
        };
        useCase
            .GetProject(query, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        // Act
        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var useCase = Substitute.For<IGetProjectsUseCase>();
        useCase
            .GetProject(query, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertProjectNotFoundProblem(response, TestUrl, "id");
    }
}

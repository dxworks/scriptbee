using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.ProjectStructure;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure;

public class GetProjectStructureEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id/files";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [InlineData(-1, 5)]
    [InlineData(6, -5)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    public async Task InvalidOffsetOrLimit_ShouldReturnBadRequest(int? offset, int? limit)
    {
        TestApiCaller<Program> api = new($"{TestUrl}?offset={offset}&limit={limit}");
        var response = await api.GetApi(new TestWebApplicationFactory<Program>(outputHelper));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertBadRequestProblem(
            response.Content,
            TestUrl,
            "Invalid pagination parameters.",
            $"Offset must be greater than or equal to 0 and limit must be greater than 0. Received offset: {offset}, limit: {limit}."
        );
    }

    [Fact]
    public async Task InvalidOffsetWithNullLimit_ShouldReturnBadRequest()
    {
        TestApiCaller<Program> api = new($"{TestUrl}?offset={-2}");
        var response = await api.GetApi(new TestWebApplicationFactory<Program>(outputHelper));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertBadRequestProblem(
            response.Content,
            TestUrl,
            "Invalid pagination parameters.",
            "Offset must be greater than or equal to 0 and limit must be greater than 0. Received offset: -2, limit: 50."
        );
    }

    [Fact]
    public async Task InvalidLimitWithNullOffset_ShouldReturnBadRequest()
    {
        TestApiCaller<Program> api = new($"{TestUrl}?limit={-3}");
        var response = await api.GetApi(new TestWebApplicationFactory<Program>(outputHelper));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertBadRequestProblem(
            response.Content,
            TestUrl,
            "Invalid pagination parameters.",
            "Offset must be greater than or equal to 0 and limit must be greater than 0. Received offset: 0, limit: -3."
        );
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IGetProjectFilesUseCase>();
        useCase
            .GetAll(
                new GetProjectFilesQuery(ProjectId.FromValue("id"), null, 0, 50),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<GetProjectFilesQueryResult, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(ProjectId.FromValue("id"))
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

    [Theory]
    [FilePath("TestData/GetProjectFiles/file.json")]
    public async Task ShouldReturnOk_WhenParentIdIsNull(string responsePath)
    {
        var useCase = Substitute.For<IGetProjectFilesUseCase>();
        var absolutePathUseCase = Substitute.For<IGetScriptAbsolutePathUseCase>();
        useCase
            .GetAll(
                new GetProjectFilesQuery(ProjectId.FromValue("id"), null, 0, 50),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<GetProjectFilesQueryResult, ProjectDoesNotExistsError>>(
                    new GetProjectFilesQueryResult(
                        [
                            new Script(
                                new ScriptId("4a382b18-da69-4b23-97fe-d9310b3dfea4"),
                                ProjectId.Create("id"),
                                new ProjectStructureFile("path"),
                                new ScriptLanguage("csharp", ".cs"),
                                []
                            ),
                        ],
                        1,
                        0,
                        50
                    )
                )
            );
        absolutePathUseCase
            .GetScriptAbsolutePath(Arg.Any<ProjectStructureEntry>())
            .Returns("absolute");

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                    services.AddSingleton(absolutePathUseCase);
                }
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetProjectFiles/folder.json")]
    public async Task ShouldReturnOk_WhenParentIdIsNotNull(string responsePath)
    {
        var useCase = Substitute.For<IGetProjectFilesUseCase>();
        var absolutePathUseCase = Substitute.For<IGetScriptAbsolutePathUseCase>();
        useCase
            .GetAll(
                new GetProjectFilesQuery(
                    ProjectId.FromValue("id"),
                    new ScriptId("0116d483-9f7a-4038-9da5-ebf98c81ce79"),
                    2,
                    3
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<GetProjectFilesQueryResult, ProjectDoesNotExistsError>>(
                    new GetProjectFilesQueryResult(
                        [
                            new ScriptFolder(
                                new ScriptId("e0ff5fb3-a310-4d8b-a4bb-b69e9f3fb3ee"),
                                ProjectId.Create("id"),
                                new ProjectStructureFile("path"),
                                [new ScriptId(Guid.NewGuid())]
                            ),
                        ],
                        66,
                        2,
                        3
                    )
                )
            );
        absolutePathUseCase
            .GetScriptAbsolutePath(Arg.Any<ProjectStructureEntry>())
            .Returns("absolute");

        TestApiCaller<Program> api = new(
            $"{TestUrl}?parentId=0116d483-9f7a-4038-9da5-ebf98c81ce79&offset={2}&limit={3}"
        );
        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                    services.AddSingleton(absolutePathUseCase);
                }
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }
}

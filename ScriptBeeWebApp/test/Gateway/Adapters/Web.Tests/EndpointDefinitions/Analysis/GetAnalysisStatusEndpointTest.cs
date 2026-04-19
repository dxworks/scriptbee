using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Analysis;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class GetAnalysisStatusEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/7662824d-6aec-43e4-8abe-2baad37d4e22";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAnalysisStatusById/in-progress.json")]
    public async Task ShouldReturnAccepted_WhenAnalysisIsRunning(string responsePath)
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("7662824d-6aec-43e4-8abe-2baad37d4e22");
        var useCase = Substitute.For<IGetAnalysisUseCase>();
        useCase
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(
                    new AnalysisInfo(
                        analysisId,
                        projectId,
                        new InstanceId("c06c7bbd-292f-4468-8049-bcbea500a98d"),
                        new ScriptId("be8e2489-3dd9-416d-af25-c69191416cf4"),
                        new FileId(Guid.NewGuid()),
                        AnalysisStatus.Running,
                        [],
                        [],
                        DateTimeOffset.Parse("2020-01-01T00:00:00Z"),
                        null
                    )
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

        await response.AssertResponse(HttpStatusCode.Accepted, responsePath);
        response
            .Headers.Location?.ToString()
            .ShouldBe($"/api/projects/project-id/analyses/{analysisId.ToString()}");
    }

    [Theory]
    [FilePath("TestData/GetAnalysisStatusById/finished.json")]
    public async Task ShouldReturnOk_WhenAnalysisIsFinished(string responsePath)
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("7662824d-6aec-43e4-8abe-2baad37d4e22");
        var useCase = Substitute.For<IGetAnalysisUseCase>();
        useCase
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(
                    new AnalysisInfo(
                        analysisId,
                        projectId,
                        new InstanceId("c06c7bbd-292f-4468-8049-bcbea500a98d"),
                        new ScriptId("be8e2489-3dd9-416d-af25-c69191416cf4"),
                        new FileId(Guid.NewGuid()),
                        AnalysisStatus.Finished,
                        [],
                        [],
                        DateTimeOffset.Parse("2050-01-01T00:00:00Z"),
                        DateTimeOffset.Parse("2050-01-04T00:00:00Z")
                    )
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

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task AnalysisNotExists_ShouldReturnNotFound()
    {
        var analysisId = new AnalysisId("7662824d-6aec-43e4-8abe-2baad37d4e22");
        var useCase = Substitute.For<IGetAnalysisUseCase>();
        useCase
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(
                    new AnalysisDoesNotExistsError(analysisId)
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

        await AssertAnalysisNotFoundProblem(
            response,
            TestUrl,
            "7662824d-6aec-43e4-8abe-2baad37d4e22"
        );
    }
}

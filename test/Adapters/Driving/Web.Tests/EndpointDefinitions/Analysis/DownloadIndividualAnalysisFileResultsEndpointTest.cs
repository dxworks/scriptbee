using System.Net;
using DxWorks.ScriptBee.Plugin.Api.Model;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class DownloadAnalysisFileResultsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/23d3df9f-786f-4b8d-b8ce-56df0e7e9e42/results/files/546d286b-d0e5-49d9-a94a-6043e9ca1da8";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnFileStreamContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("23d3df9f-786f-4b8d-b8ce-56df0e7e9e42");
        var resultId = new ResultId("546d286b-d0e5-49d9-a94a-6043e9ca1da8");
        var useCase = Substitute.For<IDownloadAnalysisFileResultsUseCase>();
        useCase
            .GetFileResultStream(projectId, analysisId, resultId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<
                        NamedFileStream,
                        AnalysisDoesNotExistsError,
                        AnalysisResultDoesNotExistsError
                    >
                >(new NamedFileStream("file.csv", new MemoryStream("content"u8.ToArray())))
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

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response
            .Content.Headers.GetValues("Content-Disposition")
            .Single()
            .ShouldContain("attachment; filename=file.csv");
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldBe("content");
    }

    [Fact]
    public async Task AnalysisNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("23d3df9f-786f-4b8d-b8ce-56df0e7e9e42");
        var resultId = new ResultId("546d286b-d0e5-49d9-a94a-6043e9ca1da8");
        var useCase = Substitute.For<IDownloadAnalysisFileResultsUseCase>();
        useCase
            .GetFileResultStream(projectId, analysisId, resultId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<
                        NamedFileStream,
                        AnalysisDoesNotExistsError,
                        AnalysisResultDoesNotExistsError
                    >
                >(new AnalysisDoesNotExistsError(analysisId))
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
            "23d3df9f-786f-4b8d-b8ce-56df0e7e9e42"
        );
    }

    [Fact]
    public async Task AnalysisResultNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("23d3df9f-786f-4b8d-b8ce-56df0e7e9e42");
        var resultId = new ResultId("546d286b-d0e5-49d9-a94a-6043e9ca1da8");
        var useCase = Substitute.For<IDownloadAnalysisFileResultsUseCase>();
        useCase
            .GetFileResultStream(projectId, analysisId, resultId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<
                        NamedFileStream,
                        AnalysisDoesNotExistsError,
                        AnalysisResultDoesNotExistsError
                    >
                >(new AnalysisResultDoesNotExistsError(resultId))
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

        await AssertAnalysisResultNotFoundProblem(
            response,
            TestUrl,
            "546d286b-d0e5-49d9-a94a-6043e9ca1da8"
        );
    }
}

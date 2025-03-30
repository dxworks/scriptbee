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

public class DownloadAllZipAnalysisFileResultsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/8c524992-b690-40e5-b1c8-00de0f78d1f9/results/files/download";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnFileStreamContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("8c524992-b690-40e5-b1c8-00de0f78d1f9");
        var useCase = Substitute.For<IDownloadAnalysisFileResultsUseCase>();
        useCase
            .GetAllFilesZipStream(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<NamedFileStream, AnalysisDoesNotExistsError>>(
                    new NamedFileStream("file.zip", new MemoryStream("content"u8.ToArray()))
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

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response
            .Content.Headers.GetValues("Content-Disposition")
            .Single()
            .ShouldContain("attachment; filename=file.zip");
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldBe("content");
    }

    [Fact]
    public async Task AnalysisNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("8c524992-b690-40e5-b1c8-00de0f78d1f9");
        var useCase = Substitute.For<IDownloadAnalysisFileResultsUseCase>();
        useCase
            .GetAllFilesZipStream(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<NamedFileStream, AnalysisDoesNotExistsError>>(
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
            "8c524992-b690-40e5-b1c8-00de0f78d1f9"
        );
    }
}

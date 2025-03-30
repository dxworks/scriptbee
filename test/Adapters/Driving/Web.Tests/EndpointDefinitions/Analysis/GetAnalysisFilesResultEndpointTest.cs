using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class GetAnalysisFilesResultEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/d9fc0cda-b40f-4ed1-8b69-f91f51682e30/results/files";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnFilesResults()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("d9fc0cda-b40f-4ed1-8b69-f91f51682e30");
        var useCase = Substitute.For<IGetAnalysisResultsUseCase>();
        useCase
            .GetFileResults(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<IEnumerable<AnalysisFileResult>, AnalysisDoesNotExistsError>>(
                    new List<AnalysisFileResult>
                    {
                        new(new FileId("04f29955-0456-46cf-be15-ef5b0d8e83bf"), "file.txt", "file"),
                    }
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
        var getResponse = await response.ReadContentAsync<WebGetAnalysisResultFileList>();
        getResponse.Files.ShouldBe(
            [
                new WebGetAnalysisResultFile(
                    "04f29955-0456-46cf-be15-ef5b0d8e83bf",
                    "file.txt",
                    "file"
                ),
            ]
        );
    }

    [Fact]
    public async Task AnalysisNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("d9fc0cda-b40f-4ed1-8b69-f91f51682e30");
        var useCase = Substitute.For<IGetAnalysisResultsUseCase>();
        useCase
            .GetFileResults(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<IEnumerable<AnalysisFileResult>, AnalysisDoesNotExistsError>>(
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
            "d9fc0cda-b40f-4ed1-8b69-f91f51682e30"
        );
    }
}

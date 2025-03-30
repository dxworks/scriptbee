using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class GetAnalysisConsoleResultEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/7e0eda67-c958-4097-903a-08917cbf987e/results/console";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnConsoleContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("7e0eda67-c958-4097-903a-08917cbf987e");
        var useCase = Substitute.For<IGetAnalysisResultsUseCase>();
        useCase
            .GetConsoleResult(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<string, AnalysisDoesNotExistsError>>("console content"));

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
        var getResponse = await response.ReadContentAsync<WebGetAnalysisResultConsole>();
        getResponse.ShouldBe(new WebGetAnalysisResultConsole("console content"));
    }

    [Fact]
    public async Task AnalysisNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("7e0eda67-c958-4097-903a-08917cbf987e");
        var useCase = Substitute.For<IGetAnalysisResultsUseCase>();
        useCase
            .GetConsoleResult(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<string, AnalysisDoesNotExistsError>>(
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
            "7e0eda67-c958-4097-903a-08917cbf987e"
        );
    }
}

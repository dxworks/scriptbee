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

public class GetAnalysisErrorsResultEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/8b7a87cd-ea21-48fb-b6bb-b6959ba21ab0/results/errors";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnErrorResults()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("8b7a87cd-ea21-48fb-b6bb-b6959ba21ab0");
        var useCase = Substitute.For<IGetAnalysisResultsUseCase>();
        useCase
            .GetErrorResults(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<IEnumerable<AnalysisErrorResult>, AnalysisDoesNotExistsError>
                >(new List<AnalysisErrorResult> { new("title", "message", "Critical") })
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
        var getResponse = await response.ReadContentAsync<WebGetAnalysisResultRunErrors>();
        getResponse.Errors.ShouldBe(
            [new WebAnalysisResultRunError("title", "message", "Critical")]
        );
    }

    [Fact]
    public async Task AnalysisNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("8b7a87cd-ea21-48fb-b6bb-b6959ba21ab0");
        var useCase = Substitute.For<IGetAnalysisResultsUseCase>();
        useCase
            .GetErrorResults(projectId, analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<IEnumerable<AnalysisErrorResult>, AnalysisDoesNotExistsError>
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

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Analysis Not Found",
            "An analysis with the ID '8b7a87cd-ea21-48fb-b6bb-b6959ba21ab0' does not exists."
        );
    }
}

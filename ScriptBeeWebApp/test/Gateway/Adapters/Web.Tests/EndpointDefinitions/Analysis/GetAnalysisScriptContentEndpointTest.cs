using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class GetAnalysisScriptContentEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/82d8c553-8855-4e7d-b070-ce2d712c6d10/scripts/2cd39efb-9a8d-4856-8ea3-356eeb2491a0/content";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task GivenScriptContent_ShouldReturnRawText()
    {
        var analysisId = new AnalysisId("82d8c553-8855-4e7d-b070-ce2d712c6d10");
        var scriptId = new ScriptId("2cd39efb-9a8d-4856-8ea3-356eeb2491a0");
        const string content = "print('hello world')";
        var useCase = Substitute.For<IGetAnalysisScriptUseCase>();
        useCase
            .GetScriptContent(analysisId, scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<string, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>
                >(content)
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
        var actualContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        actualContent.ShouldBe(content);
    }

    [Fact]
    public async Task GivenAnalysisDoesNotExist_ShouldReturnNotFound()
    {
        var analysisId = new AnalysisId("82d8c553-8855-4e7d-b070-ce2d712c6d10");
        var scriptId = new ScriptId("2cd39efb-9a8d-4856-8ea3-356eeb2491a0");
        var useCase = Substitute.For<IGetAnalysisScriptUseCase>();

        useCase
            .GetScriptContent(analysisId, scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<string, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>
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

        await AssertAnalysisNotFoundProblem(response, TestUrl, analysisId.ToString());
    }

    [Fact]
    public async Task GivenScriptDoesNotExistsError_ShouldReturnNotFound()
    {
        var analysisId = new AnalysisId("82d8c553-8855-4e7d-b070-ce2d712c6d10");
        var scriptId = new ScriptId("2cd39efb-9a8d-4856-8ea3-356eeb2491a0");
        var useCase = Substitute.For<IGetAnalysisScriptUseCase>();

        useCase
            .GetScriptContent(analysisId, scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<string, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>
                >(new ScriptDoesNotExistsError(scriptId))
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

        await AssertScriptDoesNotExistProblem(response, TestUrl, scriptId.ToString());
    }
}

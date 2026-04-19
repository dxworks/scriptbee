using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Analysis;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class GetAnalysisScriptFileEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/9be064ff-8844-48b8-9fe6-89a0a6e8ce9b/scripts/d6912b05-0297-45a2-8af3-b3541dd8dba7";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAnalysisFileScript/response.json")]
    public async Task GivenScriptMetadata_ShouldReturnOk(string responsePath)
    {
        var analysisId = new AnalysisId("9be064ff-8844-48b8-9fe6-89a0a6e8ce9b");
        var scriptId = new ScriptId("d6912b05-0297-45a2-8af3-b3541dd8dba7");
        var useCase = Substitute.For<IGetAnalysisScriptUseCase>();
        useCase
            .GetFileScript(analysisId, scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Script, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>
                >(
                    new Script(
                        scriptId,
                        ProjectId.FromValue("project-id"),
                        new ProjectStructureFile("historical-script.py"),
                        new ScriptLanguage("python", ".py"),
                        []
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
    }

    [Fact]
    public async Task GivenAnalysisDoesNotExist_ShouldReturnNotFound()
    {
        var analysisId = new AnalysisId("9be064ff-8844-48b8-9fe6-89a0a6e8ce9b");
        var scriptId = new ScriptId("d6912b05-0297-45a2-8af3-b3541dd8dba7");
        var useCase = Substitute.For<IGetAnalysisScriptUseCase>();

        useCase
            .GetFileScript(analysisId, scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Script, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>
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
    public async Task GivenScriptFileDoesNotExistError_ShouldReturnNotFound()
    {
        var analysisId = new AnalysisId("9be064ff-8844-48b8-9fe6-89a0a6e8ce9b");
        var scriptId = new ScriptId("d6912b05-0297-45a2-8af3-b3541dd8dba7");
        var useCase = Substitute.For<IGetAnalysisScriptUseCase>();

        useCase
            .GetFileScript(analysisId, scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<
                    OneOf<Script, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>
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

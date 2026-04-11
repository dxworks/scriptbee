using System.Net;
using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.ProjectStructure;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure;

using UpdateResponse = OneOf<Success, ProjectDoesNotExistsError, ScriptDoesNotExistsError>;

public class UpdateProjectScriptsContentEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/id/scripts/610ff803-4a2d-4ec1-8b63-e021c187204d/content";

    private readonly ScriptId _scriptId = new("610ff803-4a2d-4ec1-8b63-e021c187204d");
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnNoContent_WhenBodyIsSent()
    {
        var useCase = Substitute.For<IUpdateScriptUseCase>();
        useCase
            .UpdateContent(
                new UpdateScriptContentCommand(ProjectId.FromValue("id"), _scriptId, "content"),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<UpdateResponse>(new Success()));

        var response = await _api.PutApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            "content",
            MediaTypeNames.Text.Plain
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IUpdateScriptUseCase>();
        useCase
            .UpdateContent(Arg.Any<UpdateScriptContentCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<UpdateResponse>(
                    new ProjectDoesNotExistsError(ProjectId.FromValue("id"))
                )
            );

        var response = await _api.PutApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            "content",
            MediaTypeNames.Text.Plain
        );

        await AssertProjectNotFoundProblem(response, TestUrl, "id");
    }

    [Fact]
    public async Task ScriptDoesNotExistsError_ShouldReturnBadRequest()
    {
        var useCase = Substitute.For<IUpdateScriptUseCase>();
        useCase
            .UpdateContent(Arg.Any<UpdateScriptContentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<UpdateResponse>(new ScriptDoesNotExistsError(_scriptId)));

        var response = await _api.PutApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            "content",
            MediaTypeNames.Text.Plain
        );

        await AssertScriptDoesNotExistProblem(response, TestUrl, _scriptId.Value.ToString());
    }
}

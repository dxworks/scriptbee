using System.Net;
using System.Text.Json;
using DxWorks.ScriptBee.Plugin.Api.Model;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure;

using UpdateResponse = OneOf<Script, ProjectDoesNotExistsError, ScriptDoesNotExistsError>;

public class UpdateProjectScriptsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id/scripts/3283da02-5710-4b2a-bc45-496ff77be18d";
    private readonly ScriptId _scriptId = new("3283da02-5710-4b2a-bc45-496ff77be18d");
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PatchApi<WebUpdateScriptCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Theory]
    [InlineData("string", "value")]
    [InlineData("float", 12.5)]
    [InlineData("integer", 7)]
    [InlineData("boolean", true)]
    public async Task ShouldReturnOk_WhenParametersArePassed(string type, object value)
    {
        var useCase = Substitute.For<IUpdateScriptUseCase>();
        useCase
            .Update(
                Arg.Is<UpdateScriptCommand>(command =>
                    command.Parameters != null && MatchParameter(command.Parameters, type, value)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<UpdateResponse>(
                    new Script(
                        _scriptId,
                        ProjectId.Create("id"),
                        "name",
                        "path",
                        "absolute",
                        new ScriptLanguage("csharp", ".cs"),
                        [
                            new ScriptParameter
                            {
                                Name = "parameter",
                                Type = type,
                                Value = value,
                            },
                        ]
                    )
                )
            );

        var response = await _api.PatchApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebUpdateScriptCommand([new WebScriptParameter("parameter", type, value)])
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var webScriptData = await response.ReadContentAsync<WebScriptData>();
        webScriptData.Id.ShouldBe("3283da02-5710-4b2a-bc45-496ff77be18d");
        webScriptData.Name.ShouldBe("name");
        webScriptData.Path.ShouldBe("path");
        webScriptData.AbsolutePath.ShouldBe("absolute");
        webScriptData.ScriptLanguage.ShouldBe(new WebScriptLanguage("csharp", ".cs"));
        var webScriptParameter = webScriptData.Parameters.Single();
        webScriptParameter.Name.ShouldBe("parameter");
        webScriptParameter.Type.ShouldBe(type);
        ((JsonElement)webScriptParameter.Value!)
            .GetRawText()
            .ShouldBe(JsonSerializer.SerializeToElement(value).GetRawText());
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IUpdateScriptUseCase>();
        useCase
            .Update(
                new UpdateScriptCommand(ProjectId.FromValue("id"), _scriptId, null),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<UpdateResponse>(
                    new ProjectDoesNotExistsError(ProjectId.FromValue("id"))
                )
            );

        var response = await _api.PatchApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebUpdateScriptCommand(null)
        );

        await AssertProjectNotFoundProblem(response, TestUrl, "id");
    }

    [Fact]
    public async Task ScriptDoesNotExistsError_ShouldReturnBadRequest()
    {
        var useCase = Substitute.For<IUpdateScriptUseCase>();
        useCase
            .Update(
                new UpdateScriptCommand(ProjectId.FromValue("id"), _scriptId, null),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<UpdateResponse>(new ScriptDoesNotExistsError(_scriptId)));

        var response = await _api.PatchApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebUpdateScriptCommand(null)
        );

        await AssertScriptDoesNotExistProblem(response, TestUrl, _scriptId.Value.ToString());
    }

    private static bool MatchParameter(
        IEnumerable<ScriptParameter> parameters,
        string type,
        object value
    )
    {
        return parameters.Single() is { Name: "parameter" } parameter
            && parameter.Type == type
            && value.Equals(parameter.Value);
    }
}

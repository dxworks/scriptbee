using System.Net;
using System.Text.Json;
using DxWorks.ScriptBee.Plugin.Api.Model;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure;

using CreateResponse = OneOf<
    Script,
    ProjectDoesNotExistsError,
    ScriptLanguageDoesNotExistsError,
    ScriptPathAlreadyExistsError
>;

public class CreateProjectScriptsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id/scripts";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task InvalidRequestBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebCreateScriptCommand(null!, "csharp", null)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { Path = new List<string> { "'Path' must not be empty." } }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi<WebCreateScriptCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ShouldReturnCreated_WhenNoParametersArePassed()
    {
        var createScriptUseCase = Substitute.For<ICreateScriptUseCase>();
        createScriptUseCase
            .Create(
                new CreateScriptCommand(ProjectId.FromValue("id"), "path", "csharp", []),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<CreateResponse>(
                    new Script(
                        new ScriptId("7fff649a-bbd8-4570-83fb-0f8441e44999"),
                        ProjectId.Create("id"),
                        "name",
                        "path",
                        "absolute",
                        new ScriptLanguage("csharp", ".cs"),
                        []
                    )
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(createScriptUseCase);
                }
            ),
            new WebCreateScriptCommand("path", "csharp", null)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response
            .Headers.Location?.ToString()
            .ShouldBe("/api/projects/id/scripts/7fff649a-bbd8-4570-83fb-0f8441e44999");
        var webScriptData = await response.ReadContentAsync<WebScriptData>();
        webScriptData.Id.ShouldBe("7fff649a-bbd8-4570-83fb-0f8441e44999");
        webScriptData.Name.ShouldBe("name");
        webScriptData.Path.ShouldBe("path");
        webScriptData.AbsolutePath.ShouldBe("absolute");
        webScriptData.ScriptLanguage.ShouldBe(new WebScriptLanguage("csharp", ".cs"));
        webScriptData.Parameters.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("string", "value")]
    [InlineData("float", 12.5)]
    [InlineData("integer", 7)]
    [InlineData("boolean", true)]
    public async Task ShouldReturnCreated_WhenParametersArePassed(string type, object value)
    {
        var createScriptUseCase = Substitute.For<ICreateScriptUseCase>();
        createScriptUseCase
            .Create(
                Arg.Is<CreateScriptCommand>(command =>
                    MatchCreateScriptCommand(command, type, value)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<CreateResponse>(
                    new Script(
                        new ScriptId("5f33bd3b-9756-4344-8747-86afe64729ec"),
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

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(createScriptUseCase);
                }
            ),
            new WebCreateScriptCommand(
                "path",
                "csharp",
                [new WebScriptParameter("parameter", type, value)]
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response
            .Headers.Location?.ToString()
            .ShouldBe("/api/projects/id/scripts/5f33bd3b-9756-4344-8747-86afe64729ec");
        var webScriptData = await response.ReadContentAsync<WebScriptData>();
        webScriptData.Id.ShouldBe("5f33bd3b-9756-4344-8747-86afe64729ec");
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
        var createScriptUseCase = Substitute.For<ICreateScriptUseCase>();
        createScriptUseCase
            .Create(
                new CreateScriptCommand(ProjectId.FromValue("id"), "path", "csharp", []),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<CreateResponse>(
                    new ProjectDoesNotExistsError(ProjectId.FromValue("id"))
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(createScriptUseCase);
                }
            ),
            new WebCreateScriptCommand("path", "csharp", null)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Project Not Found",
            "A project with the ID 'id' does not exists."
        );
    }

    [Fact]
    public async Task ScriptLanguageNotExists_ShouldReturnBadRequest()
    {
        var createScriptUseCase = Substitute.For<ICreateScriptUseCase>();
        createScriptUseCase
            .Create(
                new CreateScriptCommand(ProjectId.FromValue("id"), "path", "csharp", []),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<CreateResponse>(new ScriptLanguageDoesNotExistsError("csharp"))
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(createScriptUseCase);
                }
            ),
            new WebCreateScriptCommand("path", "csharp", null)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { Language = new List<string> { "'csharp' language does not exists." } }
        );
    }

    [Fact]
    public async Task ExistingPath_ShouldReturnConflict()
    {
        var createScriptUseCase = Substitute.For<ICreateScriptUseCase>();
        createScriptUseCase
            .Create(
                new CreateScriptCommand(ProjectId.FromValue("id"), "path", "csharp", []),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<CreateResponse>(new ScriptPathAlreadyExistsError("path")));

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(createScriptUseCase);
                }
            ),
            new WebCreateScriptCommand("path", "csharp", null)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await AssertConflictProblem(
            response.Content,
            TestUrl,
            "Script Path Already Exists",
            "A script at that path already exists."
        );
    }

    private static bool MatchCreateScriptCommand(
        CreateScriptCommand command,
        string type,
        object value
    )
    {
        return command is { Path: "path", Language: "csharp" }
            && MatchParameter(command.Parameters, type, value);
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

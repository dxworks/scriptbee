using System.Net;
using DxWorks.ScriptBee.Plugin.Api.Model;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.ProjectStructure;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure;

public class GetProjectScriptsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id/scripts";

    [Theory]
    [FilePath("TestData/GetAllScripts/response.json")]
    public async Task ShouldReturnAllScripts(string responsePath)
    {
        var useCase = Substitute.For<IGetScriptsUseCase>();
        var absolutePathUseCase = Substitute.For<IGetScriptAbsolutePathUseCase>();
        useCase
            .GetAll(ProjectId.FromValue("id"), Arg.Any<CancellationToken>())
            .Returns([
                new Script(
                    new ScriptId("c2dd5b46-0d19-4f9e-a5bc-52d0c6522727"),
                    ProjectId.Create("id"),
                    new ProjectStructureFile("path"),
                    new ScriptLanguage("csharp", ".cs"),
                    [
                        new ScriptParameter
                        {
                            Name = "name",
                            Value = "value",
                            Type = "string",
                        },
                    ]
                ),
            ]);
        absolutePathUseCase.GetScriptAbsolutePath(Arg.Any<Script>()).Returns("absolute");

        var api = new TestApiCaller<Program>(TestUrl);
        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                    services.AddSingleton(absolutePathUseCase);
                }
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetScriptById/response.json")]
    public async Task GivenScript_ShouldReturnOk(string responsePath)
    {
        var useCase = Substitute.For<IGetScriptsUseCase>();
        var absolutePathUseCase = Substitute.For<IGetScriptAbsolutePathUseCase>();
        useCase
            .GetById(
                ProjectId.FromValue("id"),
                new ScriptId("a60eafb2-7f85-432e-891e-863bdfab59fe"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(
                    new Script(
                        new ScriptId("a60eafb2-7f85-432e-891e-863bdfab59fe"),
                        ProjectId.Create("id"),
                        new ProjectStructureFile("path"),
                        new ScriptLanguage("csharp", ".cs"),
                        [
                            new ScriptParameter
                            {
                                Name = "name",
                                Value = "value",
                                Type = "string",
                            },
                        ]
                    )
                )
            );
        absolutePathUseCase.GetScriptAbsolutePath(Arg.Any<Script>()).Returns("absolute");

        const string testUrl = $"{TestUrl}/a60eafb2-7f85-432e-891e-863bdfab59fe";
        var api = new TestApiCaller<Program>(testUrl);
        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                    services.AddSingleton(absolutePathUseCase);
                }
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Fact]
    public async Task GivenScriptDoesNotExistsError_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IGetScriptsUseCase>();
        useCase
            .GetById(
                ProjectId.FromValue("id"),
                new ScriptId("e9ca58a1-c3cf-4bc1-9252-970484c67215"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(
                    new ScriptDoesNotExistsError(
                        new ScriptId("e9ca58a1-c3cf-4bc1-9252-970484c67215")
                    )
                )
            );

        const string testUrl = $"{TestUrl}/e9ca58a1-c3cf-4bc1-9252-970484c67215";
        var api = new TestApiCaller<Program>(testUrl);
        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertScriptDoesNotExistProblem(
            response,
            testUrl,
            "e9ca58a1-c3cf-4bc1-9252-970484c67215"
        );
    }

    [Fact]
    public async Task GivenScriptContent_ShouldReturnOk()
    {
        var useCase = Substitute.For<IGetScriptsUseCase>();
        useCase
            .GetScriptContent(
                ProjectId.FromValue("id"),
                new ScriptId("04389d95-b217-45da-b690-c3f283134d9e"),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<OneOf<string, ScriptDoesNotExistsError>>("content"));

        const string testUrl = $"{TestUrl}/04389d95-b217-45da-b690-c3f283134d9e/content";
        var api = new TestApiCaller<Program>(testUrl);
        var response = await api.GetApi(
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
        actualContent.ShouldBe("content");
    }

    [Fact]
    public async Task GivenScriptDoesNotExistsError_WhenGetContent_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IGetScriptsUseCase>();
        useCase
            .GetScriptContent(
                ProjectId.FromValue("id"),
                new ScriptId("58cdce76-e1ff-4011-9d41-31138da9e94a"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<string, ScriptDoesNotExistsError>>(
                    new ScriptDoesNotExistsError(
                        new ScriptId("58cdce76-e1ff-4011-9d41-31138da9e94a")
                    )
                )
            );

        const string testUrl = $"{TestUrl}/58cdce76-e1ff-4011-9d41-31138da9e94a/content";
        var api = new TestApiCaller<Program>(testUrl);
        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertScriptDoesNotExistProblem(
            response,
            testUrl,
            "58cdce76-e1ff-4011-9d41-31138da9e94a"
        );
    }
}

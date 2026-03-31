using System.Net;
using DxWorks.ScriptBee.Plugin.Api.Model;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure;

public class GetProjectScriptsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id/scripts";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAllScripts/response.json")]
    public async Task ShouldReturnAllScripts(string responsePath)
    {
        var useCase = Substitute.For<IGetScriptsUseCase>();
        useCase
            .GetAll(ProjectId.FromValue("id"), Arg.Any<CancellationToken>())
            .Returns([
                new Script(
                    new ScriptId("c2dd5b46-0d19-4f9e-a5bc-52d0c6522727"),
                    ProjectId.Create("id"),
                    "name",
                    "path",
                    "absolute",
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
}

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Analysis;

public class RunAnalysisEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/analyses";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task InvalidRequestBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebRunAnalysisCommand("", "script-id")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { ProjectId = new List<string> { "'Project Id' must not be empty." } }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi<WebRunAnalysisCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ShouldReturnAccepted()
    {
        var runAnalysisUseCase = Substitute.For<IRunAnalysisUseCase>();
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        runAnalysisUseCase
            .Run(
                new RunAnalysisCommand(
                    ProjectId.FromValue("project-id"),
                    new ScriptId("e22be395-a668-4a26-81e7-67682afb1320")
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new AnalysisInfo(
                        new AnalysisId("3bb081e8-e453-42a1-a506-5f82bc28f0ae"),
                        ProjectId.FromValue("project-id"),
                        new ScriptId("e22be395-a668-4a26-81e7-67682afb1320"),
                        null,
                        AnalysisStatus.Started,
                        [],
                        [],
                        creationDate,
                        null
                    )
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(runAnalysisUseCase);
                }
            ),
            new WebRunAnalysisCommand("project-id", "e22be395-a668-4a26-81e7-67682afb1320")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        response
            .Headers.Location?.ToString()
            .ShouldBe("/api/analyses/3bb081e8-e453-42a1-a506-5f82bc28f0ae");
        var createProjectResponse = await response.ReadContentAsync<WebRunAnalysisResponse>();
        createProjectResponse.Id.ShouldBe("3bb081e8-e453-42a1-a506-5f82bc28f0ae");
        createProjectResponse.ProjectId.ShouldBe("project-id");
        createProjectResponse.ScriptId.ShouldBe("e22be395-a668-4a26-81e7-67682afb1320");
        createProjectResponse.Status.ShouldBe("Started");
        createProjectResponse.CreationDate.ShouldBe(creationDate);
    }
}

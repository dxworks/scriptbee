using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Analysis;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class TriggerAnalysisEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/35e67f78-ac54-44c6-b9b1-d6b0a3fa4d00/analyses";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnBadRequest_WhenPayloadIsNotValid()
    {
        var response = await _api.PostApi<WebTriggerAnalysisCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnAccepted_WhenPayloadIsValid()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("35e67f78-ac54-44c6-b9b1-d6b0a3fa4d00");
        var analysisId = Guid.NewGuid();
        var scriptId = Guid.NewGuid();
        var useCase = Substitute.For<ITriggerAnalysisUseCase>();
        useCase
            .Trigger(
                new TriggerAnalysisCommand(projectId, instanceId, new ScriptId(scriptId)),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, InstanceDoesNotExistsError>>(
                    new AnalysisInfo(
                        new AnalysisId(analysisId),
                        projectId,
                        instanceId,
                        new ScriptId(scriptId),
                        new FileId(Guid.NewGuid()),
                        AnalysisStatus.Started,
                        [],
                        [],
                        DateTime.UtcNow,
                        null
                    )
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebTriggerAnalysisCommand(scriptId.ToString())
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        response
            .Headers.Location?.ToString()
            .ShouldBe($"/api/projects/project-id/analyses/{analysisId.ToString()}");
    }

    [Fact]
    public async Task InstancesNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("35e67f78-ac54-44c6-b9b1-d6b0a3fa4d00");
        var scriptId = Guid.NewGuid();
        var useCase = Substitute.For<ITriggerAnalysisUseCase>();
        useCase
            .Trigger(
                new TriggerAnalysisCommand(projectId, instanceId, new ScriptId(scriptId)),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebTriggerAnalysisCommand(scriptId.ToString())
        );

        await AssertInstanceNotFoundProblem(
            response,
            TestUrl,
            "35e67f78-ac54-44c6-b9b1-d6b0a3fa4d00"
        );
    }
}

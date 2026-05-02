using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class DeleteAnalysisEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/analyses/c8b7fd65-d9c6-4021-8ab3-680994601463";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnNoContent_WhenDeleteIsSuccessful()
    {
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId("c8b7fd65-d9c6-4021-8ab3-680994601463");
        var useCase = Substitute.For<IDeleteAnalysisUseCase>();
        useCase
            .Delete(
                Arg.Is<DeleteAnalysisCommand>(c =>
                    c.ProjectId == projectId && c.AnalysisId == analysisId
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.CompletedTask);

        var response = await _api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase
            .Received(1)
            .Delete(
                Arg.Is<DeleteAnalysisCommand>(c =>
                    c.ProjectId == projectId && c.AnalysisId == analysisId
                ),
                Arg.Any<CancellationToken>()
            );
    }
}

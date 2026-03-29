using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class GetAllAnalysisStatusEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/analyses";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAllAnalysis/response.json")]
    public async Task ShouldReturnOk_WhenAllAnalysesAreRequested(string responsePath)
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IGetAnalysisUseCase>();
        useCase
            .GetAll(projectId, SortOrder.Descending, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<AnalysisInfo>>([
                    new AnalysisInfo(
                        new AnalysisId("7662824d-6aec-43e4-8abe-2baad37d4e22"),
                        projectId,
                        new InstanceId("c06c7bbd-292f-4468-8049-bcbea500a98d"),
                        new ScriptId("be8e2489-3dd9-416d-af25-c69191416cf4"),
                        null,
                        AnalysisStatus.Running,
                        [],
                        [],
                        DateTimeOffset.Parse("2020-01-01T00:00:00Z"),
                        null
                    ),
                ])
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
}

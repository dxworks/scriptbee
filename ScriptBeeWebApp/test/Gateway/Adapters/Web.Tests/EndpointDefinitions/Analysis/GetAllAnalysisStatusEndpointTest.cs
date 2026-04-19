using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Application.Model;
using ScriptBee.Application.Model.Sorting;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis;

public class GetAllAnalysisStatusEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/analyses";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAllAnalysis/response.json")]
    public async Task ShouldReturnOk_WhenAllAnalysesAreRequested(string responsePath)
    {
        var response = await CallApi(
            "",
            new AnalysisSort(AnalysisSortField.CreationDate, SortOrder.Descending)
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetAllAnalysis/response.json")]
    public async Task ShouldReturnOk_WhenAllAnalysesAreRequestedWithExplicitDescendingCreationDate(
        string responsePath
    )
    {
        var response = await CallApi(
            "?sort=-CreationDate",
            new AnalysisSort(AnalysisSortField.CreationDate, SortOrder.Descending)
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetAllAnalysis/response.json")]
    public async Task ShouldReturnOk_WhenAllAnalysesAreRequestedWithAscendingCreationDate(
        string responsePath
    )
    {
        var response = await CallApi(
            "?sort=CreationDate",
            new AnalysisSort(AnalysisSortField.CreationDate, SortOrder.Ascending)
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetAllAnalysis/response.json")]
    public async Task ShouldReturnOk_WhenAllAnalysesAreRequestedWithExplicitAscendingCreationDate(
        string responsePath
    )
    {
        var response = await CallApi(
            "?sort=+CreationDate",
            new AnalysisSort(AnalysisSortField.CreationDate, SortOrder.Ascending)
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    private async Task<HttpResponseMessage> CallApi(string queryParams, AnalysisSort analysisSort)
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IGetAnalysisUseCase>();
        var query = new GetAnalysisQuery(projectId, [analysisSort]);
        useCase
            .GetAll(
                Arg.Is<GetAnalysisQuery>(q =>
                    q.ProjectId == query.ProjectId
                    && q.Sort.Count == query.Sort.Count
                    && q.Sort[0].Field == query.Sort[0].Field
                    && q.Sort[0].Order == query.Sort[0].Order
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<IEnumerable<AnalysisInfo>>([CreateAnalysisInfo(projectId)]));

        var response = await _api.GetApi(
            queryParams,
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );
        return response;
    }

    private static AnalysisInfo CreateAnalysisInfo(ProjectId projectId)
    {
        return new AnalysisInfo(
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
        );
    }
}

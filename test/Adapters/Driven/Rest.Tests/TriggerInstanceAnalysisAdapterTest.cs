using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class TriggerInstanceAnalysisAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly TriggerInstanceAnalysisAdapter _triggerInstanceAnalysisAdapter = new(
        new DefaultHttpClientFactory()
    );

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task TriggerAnalysis()
    {
        _server
            .Given(Request.Create().WithPath("/api/analyses").UsingPost())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(
                        """
                        {
                          "id": "428ea7d6-6f93-487a-b749-7d58e2c5112b",
                          "projectId": "project-id",
                          "scriptId": "403e05ae-f16b-4fd7-8d23-375b1d2accb4",
                          "status": "Started",
                          "creationDate": "2024-02-08"
                        }
                        """
                    )
            );

        var analysisInfo = await _triggerInstanceAnalysisAdapter.Trigger(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            ),
            new ScriptId("403e05ae-f16b-4fd7-8d23-375b1d2accb4"),
            TestContext.Current.CancellationToken
        );

        analysisInfo.ShouldBe(
            new AnalysisInfo(
                new AnalysisId("428ea7d6-6f93-487a-b749-7d58e2c5112b"),
                ProjectId.FromValue("project-id"),
                new ScriptId("403e05ae-f16b-4fd7-8d23-375b1d2accb4"),
                null,
                AnalysisStatus.Started,
                [],
                [],
                DateTimeOffset.Parse("2024-02-08"),
                null
            )
        );
    }
}

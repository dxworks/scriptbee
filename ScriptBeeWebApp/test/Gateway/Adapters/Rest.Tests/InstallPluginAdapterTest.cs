using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class InstallPluginAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly InstallPluginAdapter _installPluginAdapter = new(
        new DefaultHttpClientFactory()
    );

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task InstallPlugin()
    {
        _server
            .Given(Request.Create().WithPath("/api/plugins").UsingPost())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(
                        """
                        {
                            "pluginId": "plugin-id",
                            "version": "1.2.3"
                        }
                        """
                    )
            );

        await _installPluginAdapter.Install(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now,
                AnalysisInstanceStatus.NotFound
            ),
            "plugin-id",
            "1.2.3",
            TestContext.Current.CancellationToken
        );

        var requests = _server.FindLogEntries(
            Request.Create().WithPath("/api/plugins").UsingPost()
        );
        requests.ShouldHaveSingleItem();
    }
}

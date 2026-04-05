using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class UninstallPluginAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly UninstallPluginAdapter _uninstallPluginAdapter = new(
        new DefaultHttpClientFactory()
    );

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task UninstallPlugin()
    {
        _server
            .Given(
                Request
                    .Create()
                    .WithPath("/api/plugins/plugin-id")
                    .WithParam("version", "1.2.3")
                    .UsingDelete()
            )
            .RespondWith(Response.Create().WithStatusCode(200));

        await _uninstallPluginAdapter.Uninstall(
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
            Request
                .Create()
                .WithPath("/api/plugins/plugin-id")
                .WithParam("version", "1.2.3")
                .UsingDelete()
        );
        requests.ShouldHaveSingleItem();
    }
}

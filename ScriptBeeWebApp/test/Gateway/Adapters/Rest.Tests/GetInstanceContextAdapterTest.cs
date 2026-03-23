using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class GetInstanceContextAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly GetInstanceContextAdapter _getInstanceContextAdapter = new(
        new DefaultHttpClientFactory()
    );

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task GetContextSlices()
    {
        _server
            .Given(Request.Create().WithPath("/api/context").UsingGet())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(
                        """
                        [
                            {
                                "model": "model",
                                "pluginIds": ["plugin-id"]
                            }
                        ]
                        """
                    )
            );

        var contextSlices = await _getInstanceContextAdapter.Get(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            ),
            TestContext.Current.CancellationToken
        );

        var slice = contextSlices.ToList().Single();
        slice.Model.ShouldBe("model");
        slice.PluginIds.ShouldBeEquivalentTo(new List<string> { "plugin-id" });
    }
}

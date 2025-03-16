using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class ClearInstanceContextAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly ClearInstanceContextAdapter _clearInstanceContextAdapter = new(
        new DefaultHttpClientFactory()
    );

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task ClearContext()
    {
        _server
            .Given(Request.Create().WithPath("/api/context/clear").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(204));

        await _clearInstanceContextAdapter.Clear(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var requests = _server.FindLogEntries(
            Request.Create().WithPath("/api/context/clear").UsingPost()
        );
        requests.ShouldHaveSingleItem();
    }
}

using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class LinkInstanceContextAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly LinkInstanceContextAdapter _linkInstanceContextAdapter = new(
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
            .Given(Request.Create().WithPath("/api/context/link").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(204));

        await _linkInstanceContextAdapter.Link(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            ),
            ["linker-id"],
            TestContext.Current.CancellationToken
        );

        var requests = _server.FindLogEntries(
            Request.Create().WithPath("/api/context/link").UsingPost()
        );
        var logEntry = requests.ShouldHaveSingleItem();
        logEntry.RequestMessage.Body!.ShouldContainWithoutWhitespace(
            """
            {
                "linkerIds": ["linker-id"]
            }
            """
        );
    }
}

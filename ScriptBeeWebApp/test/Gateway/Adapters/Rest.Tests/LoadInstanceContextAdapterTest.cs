using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class LoadInstanceContextAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly LoadInstanceContextAdapter _loadInstanceContextAdapter = new(
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
            .Given(Request.Create().WithPath("/api/context/load").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(204));

        await _loadInstanceContextAdapter.Load(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            ),
            new Dictionary<string, IEnumerable<FileId>>
            {
                { "loader-id", [new FileId("e65c0fd5-13ca-4b19-ba07-e915b17ef6ba")] },
            },
            TestContext.Current.CancellationToken
        );

        var requests = _server.FindLogEntries(
            Request.Create().WithPath("/api/context/load").UsingPost()
        );
        var logEntry = requests.ShouldHaveSingleItem();
        logEntry.RequestMessage.Body!.ShouldContainWithoutWhitespace(
            """
            {
                "filesToLoad": {
                    "loader-id": ["e65c0fd5-13ca-4b19-ba07-e915b17ef6ba"]
                }
            }
            """
        );
    }
}

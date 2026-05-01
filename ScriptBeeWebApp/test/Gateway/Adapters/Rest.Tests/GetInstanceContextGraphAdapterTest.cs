using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class GetInstanceContextGraphAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();
    private readonly GetInstanceContextGraphAdapter _adapter = new(new DefaultHttpClientFactory());

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task SearchNodes_CallsApiAndMapsResult()
    {
        // Arrange
        _server
            .Given(
                Request
                    .Create()
                    .WithPath("/api/context/graph-nodes")
                    .WithParam("query", "test")
                    .WithParam("offset", "0")
                    .WithParam("limit", "10")
                    .UsingGet()
            )
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(
                        """
                        {
                            "nodes": [
                                {
                                    "id": "node-1",
                                    "label": "Node 1",
                                    "type": "Type1",
                                    "properties": {
                                        "Prop1": "Value1"
                                    }
                                }
                            ],
                            "edges": [
                                {
                                    "source": "node-1",
                                    "target": "node-2",
                                    "label": "Edge1"
                                }
                            ]
                        }

                        """
                    )
            );

        var instanceInfo = new InstanceInfo(
            new InstanceId(Guid.Empty),
            ProjectId.FromValue("project-id"),
            _server.Urls[0],
            DateTimeOffset.Now,
            AnalysisInstanceStatus.NotFound
        );

        // Act
        var result = await _adapter.SearchNodes(
            instanceInfo,
            "test",
            0,
            10,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.Nodes.Count().ShouldBe(1);
        result.Nodes.First().Id.ShouldBe("node-1");
        result.Edges.Count().ShouldBe(1);
        result.Edges.First().Source.ShouldBe("node-1");
    }

    [Fact]
    public async Task GetNeighbors_CallsApiAndMapsResult()
    {
        // Arrange
        _server
            .Given(
                Request.Create().WithPath("/api/context/graph-nodes/node-1/neighbors").UsingGet()
            )
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(
                        """
                        {
                            "nodes": [
                                {
                                    "id": "neighbor-1",
                                    "label": "Neighbor 1",
                                    "type": "Type2",
                                    "properties": {
                                        "Prop2": "Value2"
                                    }
                                }
                            ],
                            "edges": [
                                {
                                    "source": "source-node",
                                    "target": "neighbor-1",
                                    "label": "NeighborEdge"
                                }
                            ]
                        }

                        """
                    )
            );

        var instanceInfo = new InstanceInfo(
            new InstanceId(Guid.Empty),
            ProjectId.FromValue("project-id"),
            _server.Urls[0],
            DateTimeOffset.Now,
            AnalysisInstanceStatus.NotFound
        );

        // Act
        var result = await _adapter.GetNeighbors(
            instanceInfo,
            "node-1",
            TestContext.Current.CancellationToken
        );

        // Assert
        result.Nodes.Count().ShouldBe(1);
        result.Nodes.First().Id.ShouldBe("neighbor-1");
        result.Edges.Count().ShouldBe(1);
        result.Edges.First().Target.ShouldBe("neighbor-1");
    }
}

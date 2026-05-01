using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class GetContextGraphEndpointTests(ITestOutputHelper outputHelper)
{
    private const string BaseUrl = "/api/context/graph-nodes";
    private readonly TestApiCaller<Program> _api = new(BaseUrl);

    [Theory]
    [FilePath("TestData/GetContextGraph/search-nodes-response.json")]
    public async Task SearchNodes_ReturnsOk(string responsePath)
    {
        // Arrange
        var useCase = Substitute.For<IGetContextGraphUseCase>();
        var graphResult = new ContextGraphResult(
            [
                new ContextGraphNode(
                    "node-1",
                    "Node 1",
                    "Type1",
                    null,
                    new Dictionary<string, object> { ["Prop1"] = "Value1" }
                ),
            ],
            [new ContextGraphEdge("node-1", "node-2", "Edge1")]
        );
        useCase.SearchNodes("test", 0, 10).Returns(graphResult);

        var response = await _api.GetApi(
            "?query=test&offset=0&limit=10",
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        // Assert
        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetContextGraph/get-neighbors-response.json")]
    public async Task GetNeighbors_ReturnsOk(string responsePath)
    {
        // Arrange
        var useCase = Substitute.For<IGetContextGraphUseCase>();
        var graphResult = new ContextGraphResult(
            [
                new ContextGraphNode(
                    "neighbor-1",
                    "Neighbor 1",
                    "Type2",
                    null,
                    new Dictionary<string, object> { ["Prop2"] = "Value2" }
                ),
            ],
            [new ContextGraphEdge("source-node", "neighbor-1", "NeighborEdge")]
        );
        useCase.GetNeighbors("node-1").Returns(graphResult);

        var response = await _api.GetApi(
            "/node-1/neighbors",
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        // Assert
        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }
}

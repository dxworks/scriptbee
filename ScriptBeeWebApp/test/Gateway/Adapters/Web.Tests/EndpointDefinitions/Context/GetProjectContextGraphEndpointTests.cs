using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Context;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class GetProjectContextGraphEndpointTests(ITestOutputHelper outputHelper)
{
    private const string BaseUrl =
        "/api/projects/project-id/instances/74d9ae13-525c-4076-9b5b-53303a7ad547/context/graph";
    private readonly TestApiCaller<Program> _api = new(BaseUrl);

    [Theory]
    [FilePath("TestData/GetProjectContextGraph/search-nodes-response.json")]
    public async Task SearchNodes_ReturnsOk(string responsePath)
    {
        // Arrange
        var useCase = Substitute.For<IGetInstanceContextGraphUseCase>();
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
        useCase
            .SearchNodes(
                Arg.Is<GetInstanceContextGraphQuery>(q =>
                    q.ProjectId.Value == "project-id"
                    && q.InstanceId.Value.ToString() == "74d9ae13-525c-4076-9b5b-53303a7ad547"
                    && q.Query == "test"
                    && q.Offset == 5
                    && q.Limit == 20
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<ContextGraphResult, InstanceDoesNotExistsError>>(graphResult)
            );

        var response = await _api.GetApi(
            "?query=test&offset=5&limit=20",
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        // Assert
        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetProjectContextGraph/get-neighbors-response.json")]
    public async Task GetNeighbors_ReturnsOk(string responsePath)
    {
        // Arrange
        var useCase = Substitute.For<IGetInstanceContextGraphUseCase>();
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
        useCase
            .GetNeighbors(
                Arg.Is<GetInstanceContextNeighborsQuery>(q =>
                    q.ProjectId.Value == "project-id"
                    && q.InstanceId.Value.ToString() == "74d9ae13-525c-4076-9b5b-53303a7ad547"
                    && q.NodeId == "node-1"
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<ContextGraphResult, InstanceDoesNotExistsError>>(graphResult)
            );

        var response = await _api.GetApi(
            "/neighbors?nodeId=node-1",
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        // Assert
        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Fact]
    public async Task SearchNodes_WhenInstanceDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var useCase = Substitute.For<IGetInstanceContextGraphUseCase>();
        var error = new InstanceDoesNotExistsError(
            new InstanceId("74d9ae13-525c-4076-9b5b-53303a7ad547")
        );
        useCase
            .SearchNodes(Arg.Any<GetInstanceContextGraphQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ContextGraphResult, InstanceDoesNotExistsError>>(error));

        var response = await _api.GetApi(
            "?query=test",
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}

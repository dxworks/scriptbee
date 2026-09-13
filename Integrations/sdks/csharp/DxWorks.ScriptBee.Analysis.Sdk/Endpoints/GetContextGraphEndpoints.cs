using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class GetContextGraphEndpoints : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/context/graph-nodes", SearchNodes)
            .WithTags("Context", "Graph")
            .WithSummary("Search context nodes")
            .WithDescription("Searches for context nodes based on a query string.")
            .WithName("GraphNodes");

        app.MapGet("/api/context/graph-nodes/{nodeId}/neighbors", GetNeighbors)
            .WithTags("Context", "Graph")
            .WithSummary("Get node neighbors")
            .WithDescription(
                "Retrieves the immediate neighbors and edges for a specific context node."
            )
            .WithName("Neighbors");
    }

    private static Ok<WebContextGraphResponse> SearchNodes(
        IGetContextGraphUseCase useCase,
        [FromQuery] string query = "",
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 10
    )
    {
        var result = useCase.SearchNodes(query, offset, limit);

        return TypedResults.Ok(
            new WebContextGraphResponse(
                result.Nodes.Select(WebContextGraphNode.Map),
                result.Edges.Select(WebContextGraphEdge.Map)
            )
        );
    }

    private static Ok<WebContextGraphResponse> GetNeighbors(
        [FromRoute] string nodeId,
        IGetContextGraphUseCase useCase
    )
    {
        var result = useCase.GetNeighbors(nodeId);

        return TypedResults.Ok(
            new WebContextGraphResponse(
                result.Nodes.Select(WebContextGraphNode.Map),
                result.Edges.Select(WebContextGraphEdge.Map)
            )
        );
    }
}

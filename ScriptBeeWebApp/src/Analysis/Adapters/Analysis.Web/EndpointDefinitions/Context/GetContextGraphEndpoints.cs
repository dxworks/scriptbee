using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context;

public class GetContextGraphEndpoints : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetContextGraphUseCase, GetContextGraphService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/context/graph-nodes", SearchNodes)
            .WithTags("Context", "Graph")
            .WithSummary("Search context nodes")
            .WithDescription("Searches for context nodes based on a query string.");

        app.MapGet("/api/context/graph-nodes/{nodeId}/neighbors", GetNeighbors)
            .WithTags("Context", "Graph")
            .WithSummary("Get node neighbors")
            .WithDescription(
                "Retrieves the immediate neighbors and edges for a specific context node."
            );
    }

    private static Ok<WebContextGraphResponse> SearchNodes(
        IGetContextGraphUseCase useCase,
        [FromQuery] string? query,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 10
    )
    {
        var result = useCase.SearchNodes(query ?? string.Empty, offset, limit);

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

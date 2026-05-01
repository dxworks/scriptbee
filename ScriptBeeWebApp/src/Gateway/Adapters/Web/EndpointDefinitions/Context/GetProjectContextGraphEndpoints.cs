using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Context;
using ScriptBee.UseCases.Gateway.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Context;

public class GetProjectContextGraphEndpoints : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetInstanceContextGraphUseCase, GetInstanceContextGraphService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances/{instanceId}/context/graph", SearchNodes)
            .WithTags("Instances", "Context", "Graph")
            .WithSummary("Search context nodes")
            .WithDescription("Searches for context nodes based on a query string.");

        app.MapGet(
                "/api/projects/{projectId}/instances/{instanceId}/context/graph/neighbors",
                GetNeighbors
            )
            .WithTags("Instances", "Context", "Graph")
            .WithSummary("Get node neighbors")
            .WithDescription(
                "Retrieves the immediate neighbors and edges for a specific context node."
            );
    }

    private static async Task<
        Results<Ok<WebContextGraphResponse>, NotFound<ProblemDetails>>
    > SearchNodes(
        HttpContext context,
        IGetInstanceContextGraphUseCase useCase,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        [FromQuery] string? query,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.SearchNodes(
            new GetInstanceContextGraphQuery(
                ProjectId.FromValue(projectId),
                new InstanceId(instanceId),
                query,
                offset,
                limit
            ),
            cancellationToken
        );

        return result.Match<Results<Ok<WebContextGraphResponse>, NotFound<ProblemDetails>>>(
            graphResult =>
                TypedResults.Ok(
                    new WebContextGraphResponse(
                        graphResult.Nodes.Select(WebContextGraphNode.Map),
                        graphResult.Edges.Select(WebContextGraphEdge.Map)
                    )
                ),
            error => error.ToProblem(context)
        );
    }

    private static async Task<
        Results<Ok<WebContextGraphResponse>, NotFound<ProblemDetails>>
    > GetNeighbors(
        HttpContext context,
        IGetInstanceContextGraphUseCase useCase,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        [FromQuery] string nodeId,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.GetNeighbors(
            new GetInstanceContextNeighborsQuery(
                ProjectId.FromValue(projectId),
                new InstanceId(instanceId),
                nodeId
            ),
            cancellationToken
        );

        return result.Match<Results<Ok<WebContextGraphResponse>, NotFound<ProblemDetails>>>(
            graphResult =>
                TypedResults.Ok(
                    new WebContextGraphResponse(
                        graphResult.Nodes.Select(WebContextGraphNode.Map),
                        graphResult.Edges.Select(WebContextGraphEdge.Map)
                    )
                ),
            error => error.ToProblem(context)
        );
    }
}

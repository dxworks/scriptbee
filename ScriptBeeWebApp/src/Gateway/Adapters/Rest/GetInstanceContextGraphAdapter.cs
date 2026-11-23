using Refit;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using GeneratedContracts = ScriptBee.Rest.Api.Generated.Contracts;

namespace ScriptBee.Rest;

public class GetInstanceContextGraphAdapter(IHttpClientFactory httpClientFactory)
    : IGetInstanceContextGraph
{
    public async Task<ContextGraphResult> SearchNodes(
        InstanceInfo instanceInfo,
        string? query,
        int offset,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var analysisApi = GetAnalysisApi(instanceInfo);

        var response = await analysisApi.GraphNodes(
            query ?? string.Empty,
            offset,
            limit,
            cancellationToken
        );

        return new ContextGraphResult(
            response.Nodes.Select(MapNode),
            response.Edges.Select(MapEdge)
        );
    }

    public async Task<ContextGraphResult> GetNeighbors(
        InstanceInfo instanceInfo,
        string nodeId,
        CancellationToken cancellationToken
    )
    {
        var analysisApi = GetAnalysisApi(instanceInfo);

        var response = await analysisApi.Neighbors(nodeId, cancellationToken);

        return new ContextGraphResult(
            response.Nodes.Select(MapNode),
            response.Edges.Select(MapEdge)
        );
    }

    private IAnalysisApi GetAnalysisApi(InstanceInfo instanceInfo)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        return RestService.For<IAnalysisApi>(client);
    }

    private static ContextGraphNode MapNode(GeneratedContracts.ContextGraphNode node)
    {
        return new ContextGraphNode(
            node.Id,
            node.Label,
            node.Type,
            node.Loader,
            DeserializationUtils.ConvertTo<Dictionary<string, object>>(node.Properties)
        );
    }

    private static ContextGraphEdge MapEdge(GeneratedContracts.ContextGraphEdge edge)
    {
        return new ContextGraphEdge(edge.Source, edge.Target, edge.Label);
    }
}

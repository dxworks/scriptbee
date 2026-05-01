using Refit;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;

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
        var contextApi = GetContextApi(instanceInfo);

        var response = await contextApi.SearchNodes(query, offset, limit, cancellationToken);

        return new ContextGraphResult(
            response.Nodes.Select(node => node.Map()),
            response.Edges.Select(edge => edge.Map())
        );
    }

    public async Task<ContextGraphResult> GetNeighbors(
        InstanceInfo instanceInfo,
        string nodeId,
        CancellationToken cancellationToken
    )
    {
        var contextApi = GetContextApi(instanceInfo);

        var response = await contextApi.GetNeighbors(nodeId, cancellationToken);

        return new ContextGraphResult(
            response.Nodes.Select(node => node.Map()),
            response.Edges.Select(edge => edge.Map())
        );
    }

    private IContextApi GetContextApi(InstanceInfo instanceInfo)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        return RestService.For<IContextApi>(client);
    }
}

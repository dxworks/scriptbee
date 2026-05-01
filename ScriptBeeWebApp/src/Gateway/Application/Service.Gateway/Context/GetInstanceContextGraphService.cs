using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Gateway.Context;

namespace ScriptBee.Service.Gateway.Context;

using ContextGraphResponse = OneOf<ContextGraphResult, InstanceDoesNotExistsError>;

public class GetInstanceContextGraphService(
    IGetProjectInstance getProjectInstance,
    IGetInstanceContextGraph getInstanceContextGraph
) : IGetInstanceContextGraphUseCase
{
    public async Task<ContextGraphResponse> SearchNodes(
        GetInstanceContextGraphQuery query,
        CancellationToken cancellationToken
    )
    {
        var instanceResult = await getProjectInstance.Get(query.InstanceId, cancellationToken);

        return await instanceResult.Match<Task<ContextGraphResponse>>(
            async instance =>
            {
                var result = await getInstanceContextGraph.SearchNodes(
                    instance,
                    query.Query,
                    query.Offset,
                    query.Limit,
                    cancellationToken
                );

                return result;
            },
            error => Task.FromResult<ContextGraphResponse>(error)
        );
    }

    public async Task<ContextGraphResponse> GetNeighbors(
        GetInstanceContextNeighborsQuery query,
        CancellationToken cancellationToken
    )
    {
        var instanceResult = await getProjectInstance.Get(query.InstanceId, cancellationToken);

        return await instanceResult.Match<Task<ContextGraphResponse>>(
            async instance =>
            {
                var result = await getInstanceContextGraph.GetNeighbors(
                    instance,
                    query.NodeId,
                    cancellationToken
                );

                return result;
            },
            error => Task.FromResult<ContextGraphResponse>(error)
        );
    }
}

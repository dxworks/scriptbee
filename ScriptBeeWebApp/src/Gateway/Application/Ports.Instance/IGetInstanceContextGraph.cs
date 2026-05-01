using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IGetInstanceContextGraph
{
    Task<ContextGraphResult> SearchNodes(
        InstanceInfo instanceInfo,
        string? query,
        int offset,
        int limit,
        CancellationToken cancellationToken
    );

    Task<ContextGraphResult> GetNeighbors(
        InstanceInfo instanceInfo,
        string nodeId,
        CancellationToken cancellationToken
    );
}

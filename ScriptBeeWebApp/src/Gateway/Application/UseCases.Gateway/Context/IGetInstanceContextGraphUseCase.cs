using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Context;

public record GetInstanceContextGraphQuery(
    ProjectId ProjectId,
    InstanceId InstanceId,
    string? Query,
    int Offset,
    int Limit
);

public record GetInstanceContextNeighborsQuery(
    ProjectId ProjectId,
    InstanceId InstanceId,
    string NodeId
);

public interface IGetInstanceContextGraphUseCase
{
    Task<OneOf<ContextGraphResult, InstanceDoesNotExistsError>> SearchNodes(
        GetInstanceContextGraphQuery query,
        CancellationToken cancellationToken
    );

    Task<OneOf<ContextGraphResult, InstanceDoesNotExistsError>> GetNeighbors(
        GetInstanceContextNeighborsQuery query,
        CancellationToken cancellationToken
    );
}

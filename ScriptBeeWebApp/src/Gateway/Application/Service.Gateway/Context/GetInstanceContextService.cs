using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Gateway.Context;

namespace ScriptBee.Service.Gateway.Context;

using GetInstanceContextResult = OneOf<IEnumerable<ContextSlice>, InstanceDoesNotExistsError>;

public class GetInstanceContextService(
    IGetProjectInstance getProjectInstance,
    IGetInstanceContext getInstanceContext
) : IGetInstanceContextUseCase
{
    public async Task<GetInstanceContextResult> Get(
        GetInstanceContextQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(query.InstanceId, cancellationToken);

        return await result.Match<Task<GetInstanceContextResult>>(
            async instanceInfo =>
                GetInstanceContextResult.FromT0(
                    await getInstanceContext.Get(instanceInfo, cancellationToken)
                ),
            error => Task.FromResult<GetInstanceContextResult>(error)
        );
    }
}

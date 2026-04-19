using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Gateway.Context;

namespace ScriptBee.Service.Gateway.Context;

using ClearContextResult = OneOf<Success, InstanceDoesNotExistsError>;

public class ClearInstanceContextService(
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext
) : IClearInstanceContextUseCase
{
    public async Task<ClearContextResult> Clear(
        ClearContextCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(command.InstanceId, cancellationToken);

        return await result.Match<Task<ClearContextResult>>(
            async instanceInfo =>
            {
                await clearInstanceContext.Clear(instanceInfo, cancellationToken);
                return new Success();
            },
            error => Task.FromResult<ClearContextResult>(error)
        );
    }
}

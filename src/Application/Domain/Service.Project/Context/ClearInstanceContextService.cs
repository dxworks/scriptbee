using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using ClearContextResult = OneOf<Unit, InstanceDoesNotExistsError>;

public class ClearInstanceContextService(
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext
) : IClearInstanceContextUseCase
{
    public async Task<ClearContextResult> Clear(
        ClearContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(command.InstanceId, cancellationToken);

        return await result.Match<Task<ClearContextResult>>(
            async instanceInfo =>
            {
                await clearInstanceContext.Clear(instanceInfo, cancellationToken);
                return new Unit();
            },
            error => Task.FromResult<ClearContextResult>(error)
        );
    }
}

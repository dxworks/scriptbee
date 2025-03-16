using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using ClearInstanceResult = OneOf<Unit, InstanceDoesNotExistsError>;

public class ClearInstanceContextService(
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext
) : IClearInstanceContextUseCase
{
    public async Task<ClearInstanceResult> Clear(
        ClearContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(command.InstanceId, cancellationToken);

        return await result.Match<Task<ClearInstanceResult>>(
            async instanceInfo =>
            {
                await clearInstanceContext.Clear(instanceInfo, cancellationToken);
                return new Unit();
            },
            error => Task.FromResult<ClearInstanceResult>(error)
        );
    }
}

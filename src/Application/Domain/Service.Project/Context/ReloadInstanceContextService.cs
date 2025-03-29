using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using ClearContextResult = OneOf<Unit, InstanceDoesNotExistsError>;

public class ReloadInstanceContextService(
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext
) : IReloadInstanceContextUseCase
{
    public async Task<ClearContextResult> Reload(
        ReloadContextCommand command,
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

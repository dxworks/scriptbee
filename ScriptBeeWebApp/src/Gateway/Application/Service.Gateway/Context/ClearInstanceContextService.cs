using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Gateway.Context;

namespace ScriptBee.Service.Gateway.Context;

using ClearContextResult = OneOf<Success, InstanceDoesNotExistsError>;

public class ClearInstanceContextService(
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext,
    ILogger<ClearInstanceContextService> logger
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
                logger.LogInformation(
                    "Clearing context for instance {InstanceId}",
                    instanceInfo.Id
                );
                await clearInstanceContext.Clear(instanceInfo, cancellationToken);
                logger.LogInformation("Context cleared for instance {InstanceId}", instanceInfo.Id);
                return new Success();
            },
            error => Task.FromResult<ClearContextResult>(error)
        );
    }
}

using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Gateway.Context;

public interface IUnloadInstanceContextUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Unload(
        UnloadInstanceContextCommand command,
        CancellationToken cancellationToken = default
    );
}

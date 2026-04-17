using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

public interface IClearInstanceContextUseCase
{
    Task<OneOf<Success, InstanceDoesNotExistsError>> Clear(
        ClearContextCommand command,
        CancellationToken cancellationToken
    );
}

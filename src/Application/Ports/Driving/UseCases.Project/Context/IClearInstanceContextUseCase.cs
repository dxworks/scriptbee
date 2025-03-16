using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.UseCases.Project.Context;

public interface IClearInstanceContextUseCase
{
    Task<OneOf<Unit, InstanceDoesNotExistsError>> Clear(
        ClearContextCommand command,
        CancellationToken cancellationToken = default
    );
}

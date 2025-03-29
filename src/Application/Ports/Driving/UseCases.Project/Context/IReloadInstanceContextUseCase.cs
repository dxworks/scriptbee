using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.UseCases.Project.Context;

public interface IReloadInstanceContextUseCase
{
    Task<OneOf<Unit, InstanceDoesNotExistsError>> Reload(
        ReloadContextCommand command,
        CancellationToken cancellationToken = default
    );
}

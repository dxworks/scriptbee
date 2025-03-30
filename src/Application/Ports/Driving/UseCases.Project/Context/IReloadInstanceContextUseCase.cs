using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

public interface IReloadInstanceContextUseCase
{
    Task<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Reload(
        ReloadContextCommand command,
        CancellationToken cancellationToken = default
    );
}

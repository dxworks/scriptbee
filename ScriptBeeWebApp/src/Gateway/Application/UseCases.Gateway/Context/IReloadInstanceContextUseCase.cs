using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

public interface IReloadInstanceContextUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Reload(
        ReloadContextCommand command,
        CancellationToken cancellationToken
    );
}

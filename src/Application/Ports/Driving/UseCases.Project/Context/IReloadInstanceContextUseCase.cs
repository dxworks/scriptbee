using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Context;

public interface IReloadInstanceContextUseCase
{
    Task<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Reload(
        ReloadContextCommand command,
        CancellationToken cancellationToken = default
    );
}

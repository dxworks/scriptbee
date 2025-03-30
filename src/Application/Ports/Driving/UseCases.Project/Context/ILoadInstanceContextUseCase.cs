using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

public interface ILoadInstanceContextUseCase
{
    Task<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Load(
        LoadContextCommand command,
        CancellationToken cancellationToken = default
    );
}

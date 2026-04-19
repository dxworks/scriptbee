using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Gateway.Context;

public interface ILoadInstanceContextUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Load(
        LoadContextCommand command,
        CancellationToken cancellationToken
    );
}

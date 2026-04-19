using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

public interface ILinkInstanceContextUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Link(
        LinkContextCommand command,
        CancellationToken cancellationToken
    );
}

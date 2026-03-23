using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

public interface ILinkInstanceContextUseCase
{
    Task<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Link(
        LinkContextCommand command,
        CancellationToken cancellationToken = default
    );
}

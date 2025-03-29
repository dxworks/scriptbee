using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Context;

public interface ILinkInstanceContextUseCase
{
    Task<OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Link(
        LinkContextCommand command,
        CancellationToken cancellationToken = default
    );
}

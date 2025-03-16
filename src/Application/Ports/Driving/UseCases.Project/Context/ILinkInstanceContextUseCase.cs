using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.UseCases.Project.Context;

public interface ILinkInstanceContextUseCase
{
    Task<OneOf<Unit, InstanceDoesNotExistsError>> Link(
        LinkContextCommand command,
        CancellationToken cancellationToken = default
    );
}

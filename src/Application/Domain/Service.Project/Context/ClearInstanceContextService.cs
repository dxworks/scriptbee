using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

public class ClearInstanceContextService : IClearInstanceContextUseCase
{
    public Task<OneOf<Unit, InstanceDoesNotExistsError>> Clear(
        ClearContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}

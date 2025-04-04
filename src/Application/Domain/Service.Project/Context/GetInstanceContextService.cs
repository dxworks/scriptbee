using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

public class GetInstanceContextService : IGetInstanceContextUseCase
{
    public Task<OneOf<IEnumerable<ContextSlice>, InstanceDoesNotExistsError>> Get(
        GetInstanceContextQuery query,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}

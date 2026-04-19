using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Context;

public interface IGetInstanceContextUseCase
{
    Task<OneOf<IEnumerable<ContextSlice>, InstanceDoesNotExistsError>> Get(
        GetInstanceContextQuery query,
        CancellationToken cancellationToken = default
    );
}

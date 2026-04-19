using ScriptBee.Domain.Model.Context;

namespace ScriptBee.UseCases.Project.Context;

public interface IGetInstanceLoadersUseCase
{
    Task<IEnumerable<Loader>> Get(
        GetLoadersQuery query,
        CancellationToken cancellationToken = default
    );
}

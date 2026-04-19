using ScriptBee.Domain.Model.Context;

namespace ScriptBee.UseCases.Gateway.Context;

public interface IGetInstanceLoadersUseCase
{
    Task<IEnumerable<Loader>> Get(
        GetLoadersQuery query,
        CancellationToken cancellationToken = default
    );
}

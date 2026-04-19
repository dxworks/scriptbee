using ScriptBee.Domain.Model.Context;

namespace ScriptBee.UseCases.Gateway.Context;

public interface IGetInstanceLinkersUseCase
{
    Task<IEnumerable<Linker>> Get(
        GetLinkersQuery query,
        CancellationToken cancellationToken = default
    );
}

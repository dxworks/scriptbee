using ScriptBee.Domain.Model.Context;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

public class GetInstanceLinkersService : IGetInstanceLinkersUseCase
{
    public Task<IEnumerable<Linker>> Get(
        GetLinkersQuery query,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}

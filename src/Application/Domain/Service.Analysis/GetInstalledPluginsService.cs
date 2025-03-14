using ScriptBee.Domain.Model.Plugin;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GetInstalledPluginsService : IGetInstalledPluginsUseCase
{
    public Task<IEnumerable<Plugin>> Get(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

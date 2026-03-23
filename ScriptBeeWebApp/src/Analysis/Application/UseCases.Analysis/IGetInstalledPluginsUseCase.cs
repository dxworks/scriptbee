using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.UseCases.Analysis;

public interface IGetInstalledPluginsUseCase
{
    Task<IEnumerable<Plugin>> Get(CancellationToken cancellationToken = default);
}

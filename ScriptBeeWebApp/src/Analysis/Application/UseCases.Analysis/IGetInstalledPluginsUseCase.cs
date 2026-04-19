using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.UseCases.Analysis;

public interface IGetInstalledPluginsUseCase
{
    Task<IEnumerable<Plugin>> Get(CancellationToken cancellationToken = default);
}

using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.UseCases.Analysis;

public interface IGetInstalledPluginsUseCase
{
    IEnumerable<Plugin> Get();
}

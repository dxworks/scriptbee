using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GetInstalledPluginsService(IPluginRepository pluginRepository)
    : IGetInstalledPluginsUseCase
{
    public Task<IEnumerable<Plugin>> Get(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(pluginRepository.GetLoadedPlugins());
    }
}

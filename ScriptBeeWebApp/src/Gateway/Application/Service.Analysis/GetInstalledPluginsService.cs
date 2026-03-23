using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Ports.Plugins;
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

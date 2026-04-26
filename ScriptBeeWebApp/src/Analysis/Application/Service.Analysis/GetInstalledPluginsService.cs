using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GetInstalledPluginsService(IPluginRepository pluginRepository)
    : IGetInstalledPluginsUseCase
{
    public IEnumerable<Plugin> Get()
    {
        return pluginRepository.GetLoadedPlugins();
    }
}

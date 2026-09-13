using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins.Loader;

namespace ScriptBee.Service.Analysis;

public class GetInstalledPluginsService(IPluginRepository pluginRepository)
    : IGetInstalledPluginsUseCase
{
    public IEnumerable<Plugin> Get()
    {
        return pluginRepository.GetLoadedPlugins();
    }
}

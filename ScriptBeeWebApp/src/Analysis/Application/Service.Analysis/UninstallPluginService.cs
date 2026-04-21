using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public sealed class UninstallPluginService(IPluginLoader pluginLoader) : IUninstallPluginUseCase
{
    public void UninstallPlugin(PluginId pluginId)
    {
        pluginLoader.Unload(pluginId);
    }
}

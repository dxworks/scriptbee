using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin;

public sealed class UninstallPluginService(IPluginRepository pluginRepository)
    : IUninstallPluginUseCase
{
    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        pluginRepository.UnRegisterPlugin(pluginId, pluginVersion);
    }
}

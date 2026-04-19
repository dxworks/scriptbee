using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public sealed class UninstallPluginService(IPluginRepository pluginRepository)
    : IUninstallPluginUseCase
{
    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        pluginRepository.UnRegisterPlugin(pluginId, pluginVersion);
    }
}

using ScriptBee.Plugins.Installer;
using ScriptBee.Plugins.Loader;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin;

public sealed class UninstallPluginService(
    IPluginRepository pluginRepository,
    IBundlePluginUninstaller bundlePluginUninstaller
) : IUninstallPluginUseCase
{
    public void UninstallPlugin(string pluginId, string pluginVersion)
    {
        var uninstalledPluginVersions = bundlePluginUninstaller.Uninstall(pluginId, pluginVersion);

        foreach (var (plugin, version) in uninstalledPluginVersions)
        {
            pluginRepository.UnRegisterPlugin(plugin, version);
        }
    }
}

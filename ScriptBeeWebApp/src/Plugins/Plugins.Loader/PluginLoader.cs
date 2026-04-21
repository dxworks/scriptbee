using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Loader;

internal class PluginLoader(
    ILogger<PluginLoader> logger,
    IDllLoader dllLoader,
    IPluginRegistry pluginRegistry,
    IPluginRegistrationService pluginRegistrationService
) : IPluginLoader
{
    public void Load(Plugin plugin)
    {
        foreach (var extensionPoint in plugin.Manifest.ExtensionPoints)
        {
            if (
                pluginRegistrationService.TryGetValue(
                    extensionPoint.Kind,
                    out var acceptedPluginTypes
                )
            )
            {
                if (acceptedPluginTypes!.Count == 0)
                {
                    pluginRegistry.RegisterPlugin(plugin);
                    continue;
                }

                var path = Path.Combine(plugin.FolderPath, extensionPoint.EntryPoint);

                var loadedPlugin = dllLoader.LoadDllTypes(path, acceptedPluginTypes);

                pluginRegistry.RegisterPlugin(plugin, loadedPlugin);
            }
            else
            {
                logger.LogWarning(
                    "Plugin kind '{PluginKind}' from '{EntryPoint}' has no relevant Dlls to load",
                    extensionPoint.Kind,
                    extensionPoint.EntryPoint
                );
            }
        }

        logger.LogInformation("Plugin {PluginName} loaded", plugin.Manifest.Name);
    }

    public void Unload(PluginId pluginId)
    {
        pluginRegistry.UnRegisterPlugin(pluginId);
        logger.LogInformation("Plugin {PluginId} unloaded", pluginId);
    }
}

using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Loader;

public class PluginLoader(
    ILogger<PluginLoader> logger,
    IDllLoader dllLoader,
    IPluginRepository pluginRepository,
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
                    pluginRepository.RegisterPlugin(plugin);
                    return;
                }

                var path = Path.Combine(plugin.FolderPath, extensionPoint.EntryPoint);

                var loadDllTypes = dllLoader.LoadDllTypes(path, acceptedPluginTypes).ToList();

                foreach (var (@interface, concrete) in loadDllTypes)
                {
                    pluginRepository.RegisterPlugin(plugin, @interface, concrete);
                }
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
}

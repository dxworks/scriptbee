using Microsoft.Extensions.Logging;
using ScriptBee.Common.Extensions;

namespace ScriptBee.Plugins.Installer;

public class PluginUninstaller(ILogger<PluginUninstaller> logger) : IPluginUninstaller
{
    public void Uninstall(string pathToPlugin)
    {
        var pluginName = Path.GetFileName(pathToPlugin);
        logger.LogInformation(
            "Uninstalling plugin {PluginName} from {PathToPlugin}",
            pluginName,
            pathToPlugin
        );

        new DirectoryInfo(pathToPlugin).DeleteIfExists();

        logger.LogInformation("Plugin {PluginName} uninstalled", pluginName);
    }
}

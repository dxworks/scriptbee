using Microsoft.Extensions.Logging;
using ScriptBee.Common.Extensions;

namespace ScriptBee.Plugins.Installer;

public class PluginUninstaller(ILogger<PluginUninstaller> logger) : IPluginUninstaller
{
    public void Uninstall(string pathToPlugin)
    {
        logger.LogInformation("Uninstalling plugin: {PathToPlugin}", pathToPlugin);

        new DirectoryInfo(pathToPlugin).DeleteIfExists();
    }
}

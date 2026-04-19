using Microsoft.Extensions.Options;
using ScriptBee.Plugins;
using ScriptBee.Service.Plugin.Config;

namespace ScriptBee.Service.Plugin;

public sealed class PluginPathProvider(IOptions<PluginsSettings> pluginSettings)
    : IPluginPathProvider
{
    public string GetPathToPlugins()
    {
        return pluginSettings.Value.InstallationFolder;
    }
}

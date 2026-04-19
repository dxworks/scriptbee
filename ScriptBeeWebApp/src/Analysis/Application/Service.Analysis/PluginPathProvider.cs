using Microsoft.Extensions.Options;
using ScriptBee.Plugins;
using ScriptBee.Service.Analysis.Config;

namespace ScriptBee.Service.Analysis;

public sealed class PluginPathProvider(IOptions<PluginsSettings> pluginSettings)
    : IPluginPathProvider
{
    public string GetPathToPlugins()
    {
        return pluginSettings.Value.InstallationFolder;
    }
}

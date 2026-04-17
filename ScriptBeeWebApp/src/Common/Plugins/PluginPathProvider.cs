using Microsoft.Extensions.Options;
using ScriptBee.Common.Plugins.Config;
using ScriptBee.Domain.Model.Config;

namespace ScriptBee.Common.Plugins;

public sealed class PluginPathProvider(IOptions<PluginsSettings> pluginSettings)
    : IPluginPathProvider
{
    public string GetPathToPlugins()
    {
        return pluginSettings.Value.InstallationFolder ?? ConfigFolders.PathToPlugins;
    }
}

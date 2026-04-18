using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Plugins.Installer;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Services;

public sealed class PluginPathProvider(IOptions<PluginsSettings> pluginSettings)
    : IPluginPathProvider
{
    public string GetPathToPlugins()
    {
        return pluginSettings.Value.InstallationFolder ?? ConfigFolders.PathToPlugins;
    }
}

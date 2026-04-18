using Microsoft.Extensions.Options;
using ScriptBee.Analysis.Web.Config;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Plugins.Installer;

namespace ScriptBee.Analysis.Web;

public sealed class PluginPathProvider(IOptions<PluginsSettings> pluginSettings)
    : IPluginPathProvider
{
    public string GetPathToPlugins()
    {
        return pluginSettings.Value.InstallationFolder ?? ConfigFolders.PathToPlugins;
    }
}

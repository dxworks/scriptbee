using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Project;
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

    public string GetPathToPlugins(ProjectId projectId)
    {
        return Path.Combine(ConfigFolders.PathToProjects, projectId.Value, "plugins");
    }
}

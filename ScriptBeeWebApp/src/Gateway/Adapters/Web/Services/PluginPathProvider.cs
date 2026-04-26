using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Plugins;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Services;

public sealed class PluginPathProvider(IOptions<PluginsSettings> pluginSettings)
    : IGatewayPluginPathProvider
{
    public string GetPathToPlugins()
    {
        return pluginSettings.Value.InstallationFolder ?? ConfigFolders.PathToPlugins;
    }

    public string GetInstallationFolderPath()
    {
        return pluginSettings.Value.GatewayInstallationFolder ?? ConfigFolders.PathToGatewayPlugins;
    }

    public string GetPathToPlugins(ProjectId projectId)
    {
        return Path.Combine(ConfigFolders.PathToProjects, projectId.Value, "plugins");
    }
}

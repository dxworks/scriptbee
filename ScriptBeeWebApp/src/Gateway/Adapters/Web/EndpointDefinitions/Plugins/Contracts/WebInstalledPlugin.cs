using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebInstalledPlugin(string Id, string Version)
{
    public static WebInstalledPlugin Map(PluginInstallationConfig pluginInstallationConfig)
    {
        return new WebInstalledPlugin(
            pluginInstallationConfig.PluginId,
            pluginInstallationConfig.Version.ToString()
        );
    }
}

using System.ComponentModel;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

[Description("Represents an installed plugin with its ID and version.")]
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

using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebPluginVersion(string Url, string Version, string ManifestUrl)
{
    public static WebPluginVersion Map(PluginVersion version)
    {
        return new WebPluginVersion(version.Url, version.Version.ToString(), version.ManifestUrl);
    }
}

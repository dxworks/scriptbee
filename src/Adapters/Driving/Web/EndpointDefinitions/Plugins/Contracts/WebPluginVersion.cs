using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebPluginVersion(string Version)
{
    public static WebPluginVersion Map(MarketPlacePluginVersion version)
    {
        return new WebPluginVersion(version.Version.ToString());
    }
}

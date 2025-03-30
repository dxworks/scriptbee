using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Web.EndpointDefinitions.PluginMarketPlace.Contracts;

public sealed record WebPluginVersion(string Version, bool Installed)
{
    public static WebPluginVersion Map(MarketPlacePluginVersion version)
    {
        return new WebPluginVersion(version.Version.ToString(), version.Installed);
    }
}

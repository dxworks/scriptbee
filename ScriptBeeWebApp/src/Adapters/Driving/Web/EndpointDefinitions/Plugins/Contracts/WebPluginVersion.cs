using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebPluginVersion(string Version, bool Installed, DateTime PublishDate)
{
    public static WebPluginVersion Map(MarketPlacePluginVersion version)
    {
        return new WebPluginVersion(version.Version.ToString(), version.Installed, DateTime.UtcNow);
    }
}

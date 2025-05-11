using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebMarketplaceProject(
    string Id,
    string Name,
    string Type,
    string Description,
    List<string> Authors,
    List<WebPluginVersion> Versions
)
{
    public const string PluginType = "Plugin";
    public const string BundleType = "Bundle";

    public static WebMarketplaceProject Map(MarketPlacePluginEntry marketPlacePluginEntry)
    {
        var plugin = marketPlacePluginEntry.Plugin;
        var type = plugin.Type == MarketPlacePluginType.Plugin ? PluginType : BundleType;

        return new WebMarketplaceProject(
            plugin.Id,
            plugin.Name,
            type,
            plugin.Description,
            plugin.Authors,
            marketPlacePluginEntry.InstalledVersions.Select(WebPluginVersion.Map).ToList()
        );
    }
}

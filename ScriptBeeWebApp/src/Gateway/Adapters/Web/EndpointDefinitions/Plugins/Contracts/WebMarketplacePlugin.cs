using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebMarketplacePlugin(
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

    public static WebMarketplacePlugin Map(MarketPlacePlugin marketPlacePlugin)
    {
        var type = marketPlacePlugin.Type == MarketPlacePluginType.Plugin ? PluginType : BundleType;

        return new WebMarketplacePlugin(
            marketPlacePlugin.Id,
            marketPlacePlugin.Name,
            type,
            marketPlacePlugin.Description,
            marketPlacePlugin.Authors,
            marketPlacePlugin.Versions.Select(WebPluginVersion.Map).ToList()
        );
    }
}

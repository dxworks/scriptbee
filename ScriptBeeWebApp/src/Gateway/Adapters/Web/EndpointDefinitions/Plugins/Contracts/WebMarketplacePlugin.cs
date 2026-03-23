using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.MarketPlace;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebMarketplacePlugin(
    string Id,
    string Name,
    string Type,
    string Description,
    List<string> Authors,
    string? LatestVersion,
    string? InstalledVersion
)
{
    public const string PluginType = "Plugin";
    public const string BundleType = "Bundle";

    public static WebMarketplacePlugin Map(MarketPlacePluginEntry marketPlacePluginEntry)
    {
        var plugin = marketPlacePluginEntry.Plugin;
        var type = plugin.Type == MarketPlacePluginType.Plugin ? PluginType : BundleType;

        var latestVersion = marketPlacePluginEntry.InstalledVersions.MaxBy(v => v.Version)?.Version;
        var installedVersion = marketPlacePluginEntry
            .InstalledVersions.FirstOrDefault(v => v.Installed)
            ?.Version;

        return new WebMarketplacePlugin(
            plugin.Id,
            plugin.Name,
            type,
            plugin.Description,
            plugin.Authors,
            latestVersion?.ToString(),
            installedVersion?.ToString()
        );
    }
}

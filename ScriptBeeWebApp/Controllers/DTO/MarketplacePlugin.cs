using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.DTO;

public abstract record BaseMarketplacePlugin(
    string Id,
    string Name,
    string Description,
    string Author,
    string DownloadUrl,
    string Type
)
{
    protected const string PluginType = "plugin";
    protected const string BundleType = "bundle";
}

public sealed record MarketplacePlugin(
    string Id,
    string Name,
    string Description,
    string Author,
    string DownloadUrl,
    Dictionary<string, PluginVersion> Versions
) : BaseMarketplacePlugin(Id, Name, Description, Author, DownloadUrl, PluginType);

public sealed record PluginVersion(List<string> Kinds, bool Installed);

public sealed record MarketplaceBundlePlugin(
    string Id,
    string Name,
    string Description,
    string Author,
    string DownloadUrl,
    Dictionary<string, BundlePluginVersion> Versions
) : BaseMarketplacePlugin(Id, Name, Description, Author, DownloadUrl, BundleType);

public sealed record BundlePlugin(string Name, string Version, List<string> Kinds);

public sealed record BundlePluginVersion(List<BundlePlugin> Plugins, bool Installed);

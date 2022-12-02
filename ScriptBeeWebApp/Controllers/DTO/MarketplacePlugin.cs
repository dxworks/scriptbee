using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.DTO;

public sealed record MarketplacePlugin(
    string Id,
    string Name,
    string Author,
    string Description,
    Dictionary<string, PluginVersion> Versions
);

public sealed record PluginVersion(List<ExtensionPointVersion> ExtensionPointVersions, bool Installed);

public sealed record ExtensionPointVersion(string Kind, string Version);

// public sealed record MarketplaceBundlePlugin(
//     string Id,
//     string Name,
//     string Description,
//     string Author,
//     string DownloadUrl,
//     Dictionary<string, BundlePluginVersion> Versions
// ) : BaseMarketplacePlugin(Id, Name, Description, Author, DownloadUrl, BundleType);
//
// public sealed record BundlePlugin(string Name, string Version, List<string> Kinds);
//
// public sealed record BundlePluginVersion(List<BundlePlugin> Plugins, bool Installed);

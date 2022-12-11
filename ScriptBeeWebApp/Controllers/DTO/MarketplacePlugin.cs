using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.DTO;

public sealed record MarketplacePlugin(
    string Id,
    string Name,
    string Description,
    List<string> Authors,
    List<PluginVersion> Versions
);

public sealed record PluginVersion(string Version, List<ExtensionPointVersion> ExtensionPointVersions, bool Installed);

public sealed record ExtensionPointVersion(string Kind, string Version);

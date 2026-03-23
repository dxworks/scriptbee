namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebBundleItem(string Id, string Name);

public sealed record WebMarketplacePluginWithDetails(
    string Id,
    string Name,
    string Type,
    string Description,
    List<string> Authors,
    List<WebPluginVersion> Versions,
    List<WebBundleItem>? BundleItems = null,
    string? SourceCode = null,
    string? Manifest = null,
    string? Site = null,
    string? License = null,
    List<string>? Tags = null,
    List<string>? Technologies = null,
    List<WebExtensionPoint>? ExtensionPoints = null
);

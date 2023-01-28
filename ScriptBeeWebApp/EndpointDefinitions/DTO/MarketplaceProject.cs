namespace ScriptBeeWebApp.EndpointDefinitions.DTO;

public sealed record MarketplaceProject(
    string Id,
    string Name,
    string Type,
    string Description,
    List<string> Authors,
    List<PluginVersion> Versions
)
{
    public const string PluginType = "Plugin";
    public const string BundleType = "Bundle";
}

public sealed record PluginVersion(string Version, bool Installed);

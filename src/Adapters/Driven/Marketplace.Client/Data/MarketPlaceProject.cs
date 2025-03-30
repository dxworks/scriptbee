namespace ScriptBee.Marketplace.Client.Data;

public enum MarketPlaceProjectType
{
    Plugin,
    Bundle,
}

public record MarketPlaceProject(
    string Id,
    string Name,
    MarketPlaceProjectType Type,
    string Description,
    List<string> Authors,
    List<PluginVersion> Versions
);

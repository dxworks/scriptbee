namespace ScriptBee.Domain.Model.Plugin;

public record MarketPlacePlugin(
    string Id,
    string Name,
    MarketPlaceProjectType Type,
    string Description,
    List<string> Authors,
    List<PluginVersion> Versions
);

namespace ScriptBee.Domain.Model.Plugin.MarketPlace;

public record MarketPlacePlugin(
    string Id,
    string Name,
    MarketPlacePluginType Type,
    string Description,
    List<string> Authors,
    List<PluginVersion> Versions
);

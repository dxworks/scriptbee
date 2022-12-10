namespace ScriptBee.Marketplace.Client.Data;

public record MarketPlacePlugin(
    string Id,
    string Name,
    string Description,
    List<string> Authors,
    List<PluginVersion> Versions
);

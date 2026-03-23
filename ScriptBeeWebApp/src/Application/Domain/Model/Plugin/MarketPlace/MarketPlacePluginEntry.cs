namespace ScriptBee.Domain.Model.Plugin.MarketPlace;

public record MarketPlacePluginEntry(
    MarketPlacePlugin Plugin,
    IEnumerable<MarketPlacePluginVersion> InstalledVersions
);

namespace ScriptBee.Marketplace.Client.Data;

public record PluginVersion(
    string Url,
    Version Version,
    List<ExtensionPointVersion> ExtensionPoints
);

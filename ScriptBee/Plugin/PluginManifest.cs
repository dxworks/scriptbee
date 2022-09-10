namespace ScriptBee.Plugin;

public record PluginManifest(
    string Name,
    string Version,
    string Description,
    string Author,
    string EntryPoint,
    string Type,
    object? Specs
);

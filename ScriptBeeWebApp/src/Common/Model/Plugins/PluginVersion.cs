namespace ScriptBee.Domain.Model.Plugins;

public sealed record PluginVersion(string Url, Version Version, string ManifestUrl);

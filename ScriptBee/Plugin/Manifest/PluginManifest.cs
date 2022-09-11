namespace ScriptBee.Plugin.Manifest;

public abstract class PluginManifest
{
    public string ApiVersion { get; set; } = ""; // todo use it
    public string Kind { get; set; } = "";
    public PluginManifestMetadata Metadata { get; set; } = new();
}

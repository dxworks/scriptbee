namespace ScriptBee.Plugin.Manifest;

public class UiPluginManifest : PluginManifest
{
    public UiPluginManifestSpec Spec { get; set; } = new();
}

public class UiPluginManifestSpec
{
    public string RemoteEntry { get; set; } = "";
    public string RemoteName { get; set; } = "";
    public string ExposedModule { get; set; } = "";
    public string ComponentName { get; set; } = "";
    public string UiPluginType { get; set; } = "";
}

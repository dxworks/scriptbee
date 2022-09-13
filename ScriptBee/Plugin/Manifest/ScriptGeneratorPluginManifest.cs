namespace ScriptBee.Plugin.Manifest;

public class ScriptGeneratorPluginManifest : PluginManifest
{
    public ScriptGeneratorSpec Spec { get; set; } = new();
}

public class ScriptGeneratorSpec
{
    public string Language { get; set; } = "";
    public string Extension { get; set; } = "";
}

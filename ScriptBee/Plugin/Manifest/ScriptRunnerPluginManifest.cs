namespace ScriptBee.Plugin.Manifest;

public class ScriptRunnerPluginManifest : PluginManifest
{
    public ScriptRunnerSpec Spec { get; set; } = new();
}

public class ScriptRunnerSpec
{
    public string Language { get; set; } = "";
}

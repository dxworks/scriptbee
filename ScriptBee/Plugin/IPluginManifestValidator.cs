namespace ScriptBee.Plugin;

public interface IPluginManifestValidator
{
    bool Validate(PluginManifest manifest);
}

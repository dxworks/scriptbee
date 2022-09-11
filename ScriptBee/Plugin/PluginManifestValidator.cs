using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public class PluginManifestValidator : IPluginManifestValidator
{
    public bool Validate(PluginManifest manifest)
    {
        return !string.IsNullOrEmpty(manifest.Metadata.EntryPoint);
    }
}

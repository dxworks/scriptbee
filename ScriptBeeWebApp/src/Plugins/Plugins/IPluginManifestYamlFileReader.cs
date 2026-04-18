using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Plugins;

public interface IPluginManifestYamlFileReader
{
    PluginManifest Read(string filePath);
}

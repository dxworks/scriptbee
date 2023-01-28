using ScriptBee.Plugin.Manifest;

namespace ScriptBee.FileManagement;

// todo try to generify
public interface IPluginManifestYamlFileReader
{
    PluginManifest Read(string filePath);
}

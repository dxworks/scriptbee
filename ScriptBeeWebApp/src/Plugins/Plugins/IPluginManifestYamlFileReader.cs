using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Plugins;

public interface IPluginManifestYamlFileReader
{
    PluginManifest Read(string filePath);
}

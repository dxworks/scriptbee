using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Persistence.File.Plugin;

public interface IPluginManifestYamlFileReader
{
    PluginManifest Read(string filePath);
}

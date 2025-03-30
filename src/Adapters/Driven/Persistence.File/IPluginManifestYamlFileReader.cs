using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Persistence.File;

public interface IPluginManifestYamlFileReader
{
    PluginManifest Read(string filePath);
}

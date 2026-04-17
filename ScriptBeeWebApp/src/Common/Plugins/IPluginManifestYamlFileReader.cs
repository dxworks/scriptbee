using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Common.Plugins;

public interface IPluginManifestYamlFileReader
{
    PluginManifest Read(string filePath);
}

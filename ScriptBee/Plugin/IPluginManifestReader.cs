using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public interface IPluginManifestReader
{
    IEnumerable<PluginManifest> ReadManifests(string pluginFolderPath);
}

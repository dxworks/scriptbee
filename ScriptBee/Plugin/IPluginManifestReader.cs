using System.Collections.Generic;

namespace ScriptBee.Plugin;

public interface IPluginManifestReader
{
    IEnumerable<PluginManifest> ReadManifests(string pluginFolderPath);
}

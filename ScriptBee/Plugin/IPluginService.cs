using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

// todo maybe combine with plugin repository
public interface IPluginService
{
    void Add(PluginManifest pluginManifest);

    IEnumerable<PluginManifest> GetLoadedPlugins();
}

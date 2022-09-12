using System.Collections.Concurrent;
using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public class PluginService : IPluginService
{
    private readonly ConcurrentBag<PluginManifest> _pluginManifests = new();

    public void Add(PluginManifest pluginManifest)
    {
        _pluginManifests.Add(pluginManifest);
    }

    public IEnumerable<PluginManifest> GetLoadedPlugins()
    {
        return _pluginManifests;
    }
}

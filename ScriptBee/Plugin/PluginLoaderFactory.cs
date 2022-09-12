using System.Collections.Generic;
using System.Linq;

namespace ScriptBee.Plugin;

public class PluginLoaderFactory : IPluginLoaderFactory
{
    private readonly IEnumerable<IPluginLoader> _pluginLoaders;

    public PluginLoaderFactory(IEnumerable<IPluginLoader> pluginLoaders)
    {
        _pluginLoaders = pluginLoaders;
    }

    public IPluginLoader? GetPluginLoader(Models.Plugin plugin)
    {
        // todo if plugin is helper functions or script runner, the instantiation should be done in the run script controller

        return _pluginLoaders.FirstOrDefault(loader => loader.AcceptedPluginKind == plugin.Manifest.Kind);
    }
}

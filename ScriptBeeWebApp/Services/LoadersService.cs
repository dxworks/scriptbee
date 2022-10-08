using System.Collections.Generic;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class LoadersService : ILoadersService
{
    private readonly IPluginRepository _pluginRepository;

    public LoadersService(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public IEnumerable<string> GetSupportedLoaders()
    {
        return _pluginRepository.GetLoadedPluginManifests()
            .Where(manifest => manifest.ExtensionPoints.Any(extensionPoint => extensionPoint.Kind == PluginKind.Loader))
            .Select(manifest => manifest.Name);
    }

    public IModelLoader? GetLoader(string name)
    {
        return _pluginRepository.GetPlugin<IModelLoader>(strategy => strategy.GetName() == name);
    }

    public ISet<string> GetAcceptedModules()
    {
        var acceptedModules = new HashSet<string>();

        foreach (var modelLoader in _pluginRepository.GetPlugins<IModelLoader>())
            acceptedModules.Add(modelLoader.GetType().Module.Name);

        return acceptedModules;
    }
}

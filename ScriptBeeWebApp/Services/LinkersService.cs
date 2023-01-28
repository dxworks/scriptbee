using System.Collections.Generic;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;

namespace ScriptBeeWebApp.Services;

public class LinkersService : ILinkersService
{
    private readonly IPluginRepository _pluginRepository;

    public LinkersService(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public IEnumerable<string> GetSupportedLinkers()
    {
        return _pluginRepository.GetLoadedPluginManifests()
            .Where(manifest => manifest.ExtensionPoints.Any(extensionPoint => extensionPoint.Kind == PluginKind.Linker))
            .Select(manifest => manifest.Name);
    }

    public IModelLinker? GetLinker(string name)
    {
        return _pluginRepository.GetPlugin<IModelLinker>(strategy => strategy.GetName() == name);
    }
}

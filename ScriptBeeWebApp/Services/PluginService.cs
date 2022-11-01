using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers.DTO;

namespace ScriptBeeWebApp.Services;

public sealed class PluginService : IPluginService
{
    private readonly IPluginRepository _pluginRepository;


    public PluginService(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public IEnumerable<PluginManifest> GetPluginManifests()
    {
        return _pluginRepository.GetLoadedPluginManifests();
    }

    public IEnumerable<PluginManifest> GetPluginManifests(string type)
    {
        return _pluginRepository.GetLoadedPluginManifests().Where(manifest =>
            manifest.ExtensionPoints.Any(extensionPoint => extensionPoint.Kind == type));
    }

    public IEnumerable<T> GetExtensionPoints<T>() where T : PluginExtensionPoint
    {
        return _pluginRepository.GetLoadedPluginExtensionPoints<T>();
    }

    public IEnumerable<BaseMarketplacePlugin> GetMarketPlugins(int start, int count)
    {
        for (var i = start; i < start + count; i++)
        {
            yield return new MarketplacePlugin(
                i.ToString(),
                $"Plugin {i}",
                $"Author {i}",
                $"Description {i}",
                $"DownloadUrl {i}",
                new Dictionary<string, PluginVersion>
                {
                    {
                        "1.0.0", new PluginVersion(new List<string>
                        {
                            "ScriptRunner", "ScriptGenerator"
                        }, false)
                    },
                    {
                        "1.0.1", new PluginVersion(new List<string>
                        {
                            "ScriptRunner", "ScriptGenerator", "HelperFunctions"
                        }, true)
                    }
                }
            );
        }

        for (var i = start; i < start + count; i++)
        {
            yield return new MarketplaceBundlePlugin(
                i.ToString(),
                $"Bundle {i}",
                $"Author {i}",
                $"Description {i}",
                $"DownloadUrl {i}",
                new Dictionary<string, BundlePluginVersion>
                {
                    {
                        "1.0.0", new BundlePluginVersion(new List<BundlePlugin>
                        {
                            new("Plugin 1", "1.0.0", new List<string> { "ScriptRunner", "ScriptGenerator" }),
                            new("Plugin 2", "1.0.0",
                                new List<string> { "ScriptRunner", "ScriptGenerator", "HelperFunctions" }),
                        }, true)
                    },
                    {
                        "1.0.1", new BundlePluginVersion(new List<BundlePlugin>
                        {
                            new("Plugin 1", "1.0.1", new List<string> { "ScriptRunner", "ScriptGenerator" }),
                            new("Plugin 2", "1.0.1",
                                new List<string> { "ScriptRunner", "ScriptGenerator", "HelperFunctions" }),
                            new("Plugin 3", "1.0.1",
                                new List<string> { "ScriptRunner", "ScriptGenerator", "HelperFunctions" }),
                        }, false)
                    },
                }
            );
        }
    }

    public Task InstallPlugin(string pluginId, string downloadUrl)
    {
        throw new NotImplementedException();
    }

    public Task UninstallPlugin(string pluginId)
    {
        throw new NotImplementedException();
    }
}

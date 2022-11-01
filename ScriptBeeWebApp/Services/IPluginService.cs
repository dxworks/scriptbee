using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers.DTO;

namespace ScriptBeeWebApp.Services;

public interface IPluginService
{
    IEnumerable<PluginManifest> GetPluginManifests();
    IEnumerable<PluginManifest> GetPluginManifests(string type);
    IEnumerable<T> GetExtensionPoints<T>() where T : PluginExtensionPoint;
    IEnumerable<BaseMarketplacePlugin> GetMarketPlugins(int start, int count);
    Task InstallPlugin(string pluginId, string downloadUrl);
    Task UninstallPlugin(string pluginId);
}

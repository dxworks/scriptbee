using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Ports.Plugins;

public interface IPluginRepository
{
    void UnRegisterPlugin(string pluginId, string pluginVersion);

    void RegisterPlugin(Plugin plugin, Type @interface, Type concrete);

    void RegisterPlugin(Plugin plugin);

    TService? GetPlugin<TService>(
        Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null
    )
        where TService : IPlugin;

    IEnumerable<TService> GetPlugins<TService>(
        IEnumerable<(Type @interface, object instance)>? services = null
    )
        where TService : IPlugin;

    IEnumerable<PluginManifest> GetLoadedPluginsManifests();

    IEnumerable<Plugin> GetLoadedPlugins(string kind);

    IEnumerable<T> GetLoadedPluginExtensionPoints<T>()
        where T : PluginExtensionPoint;

    Version? GetInstalledPluginVersion(string pluginId);

    IEnumerable<Plugin> GetLoadedPlugins();
}

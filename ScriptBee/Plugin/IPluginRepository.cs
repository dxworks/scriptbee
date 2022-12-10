using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public interface IPluginRepository
{
    void UnRegisterPlugin(string pluginId, string pluginVersion);

    void RegisterPlugin(Models.Plugin plugin, Type @interface, Type concrete);

    void RegisterPlugin(Models.Plugin plugin);

    TService? GetPlugin<TService>(Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : IPlugin;

    IEnumerable<TService> GetPlugins<TService>(IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : IPlugin;

    IEnumerable<PluginManifest> GetLoadedPluginsManifests();

    IEnumerable<Models.Plugin> GetLoadedPlugins(string kind);

    IEnumerable<T> GetLoadedPluginExtensionPoints<T>()
        where T : PluginExtensionPoint;

    Version? GetInstalledPluginVersion(string pluginId);
}

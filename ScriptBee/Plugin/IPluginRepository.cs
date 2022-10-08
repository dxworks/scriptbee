using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public interface IPluginRepository
{
    void RegisterPlugin(PluginManifest pluginManifest, Type @interface, Type concrete);

    void RegisterPlugin(PluginManifest pluginManifest);

    TService? GetPlugin<TService>(Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : IPlugin;

    IEnumerable<TService> GetPlugins<TService>(IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : IPlugin;

    PluginManifest? GetLoadedPluginManifest(string name);

    IEnumerable<PluginManifest> GetLoadedPluginManifests();

    IEnumerable<T> GetLoadedPluginExtensionPoints<T>()
        where T : PluginExtensionPoint;
}

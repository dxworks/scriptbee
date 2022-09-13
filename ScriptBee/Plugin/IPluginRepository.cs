using System;
using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public interface IPluginRepository
{
    void RegisterPlugin(PluginManifest pluginManifest, Type @interface, Type concrete);

    void RegisterPlugin(PluginManifest pluginManifest);

    TService? GetPlugin<TService>(Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : class;

    IEnumerable<TService> GetPlugins<TService>(IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : class;

    IEnumerable<T> GetLoadedPlugins<T>()
        where T : PluginManifest;
}

using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public interface IPluginRepository
{
    void UnRegisterPlugin(string pluginId, string pluginVersion);

    void RegisterPlugin(string pluginId, Version pluginVersion, PluginManifest pluginManifest, Type @interface,
        Type concrete);

    void RegisterPlugin(string pluginId, Version pluginVersion, PluginManifest pluginManifest);

    TService? GetPlugin<TService>(Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : IPlugin;

    IEnumerable<TService> GetPlugins<TService>(IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : IPlugin;

    IEnumerable<PluginManifest> GetLoadedPluginManifests();

    IEnumerable<T> GetLoadedPluginExtensionPoints<T>()
        where T : PluginExtensionPoint;

    Version? GetInstalledPluginVersion(string pluginName);
}

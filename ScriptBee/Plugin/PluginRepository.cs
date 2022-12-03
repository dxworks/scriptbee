using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

// todo add tests
public class PluginRepository : IPluginRepository
{
    private readonly ConcurrentDictionary<string, (Version version, PluginManifest manifest)> _pluginManifests = new();
    private readonly ConcurrentBag<ServiceDescriptor> _pluginServiceCollection = new();

    public void UnRegisterPlugin(string pluginId, string pluginVersion)
    {
        _pluginManifests.Remove(pluginId, out _);
    }

    public void RegisterPlugin(string pluginId, Version pluginVersion, PluginManifest pluginManifest, Type @interface,
        Type concrete)
    {
        RegisterPlugin(pluginId, pluginVersion, pluginManifest);

        _pluginServiceCollection.Add(new ServiceDescriptor(@interface, concrete, ServiceLifetime.Singleton));
    }

    public void RegisterPlugin(string pluginId, Version pluginVersion, PluginManifest pluginManifest)
    {
        _pluginManifests.AddOrUpdate(pluginId, (pluginVersion, pluginManifest),
            (_, manifest) => manifest.version < pluginVersion ? (pluginVersion, pluginManifest) : manifest);
    }

    public TService? GetPlugin<TService>(Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null) where TService : IPlugin
    {
        return GetPlugins<TService>().FirstOrDefault(filter);
    }

    public IEnumerable<TService> GetPlugins<TService>(IEnumerable<(Type @interface, object instance)>? services = null)
        where TService : IPlugin
    {
        var serviceCollection = new ServiceCollection
        {
            _pluginServiceCollection
        };

        if (services is not null)
            serviceCollection.Add(services.Select(s => new ServiceDescriptor(s.@interface, s.instance)));

        return serviceCollection.BuildServiceProvider()
            .GetServices<TService>();
    }

    public IEnumerable<PluginManifest> GetLoadedPluginManifests()
    {
        return _pluginManifests.Values.Select(tuple => tuple.manifest);
    }

    public IEnumerable<T> GetLoadedPluginExtensionPoints<T>() where T : PluginExtensionPoint
    {
        return GetLoadedPluginManifests().SelectMany(p => p.ExtensionPoints).OfType<T>();
    }

    public Version? GetInstalledPluginVersion(string pluginName)
    {
        return _pluginManifests.TryGetValue(pluginName, out var tuple) ? tuple.version : null;
    }
}

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
    private readonly ConcurrentBag<PluginManifest> _pluginManifests = new();
    private readonly ConcurrentBag<ServiceDescriptor> _pluginServiceCollection = new();

    public void RegisterPlugin(PluginManifest pluginManifest, Type @interface, Type concrete)
    {
        _pluginManifests.Add(pluginManifest);

        _pluginServiceCollection.Add(new ServiceDescriptor(@interface, concrete, ServiceLifetime.Singleton));
    }

    public void RegisterPlugin(PluginManifest pluginManifest)
    {
        _pluginManifests.Add(pluginManifest);
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

    public PluginManifest? GetLoadedPluginManifest(string name)
    {
        return GetLoadedPluginManifests().FirstOrDefault(manifest => manifest.Name == name);
    }

    public IEnumerable<PluginManifest> GetLoadedPluginManifests()
    {
        return _pluginManifests;
    }

    public IEnumerable<T> GetLoadedPluginExtensionPoints<T>() where T : PluginExtensionPoint
    {
        return _pluginManifests.SelectMany(p => p.ExtensionPoints).OfType<T>();
    }
}

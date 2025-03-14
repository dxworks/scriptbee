using System.Collections.Concurrent;
using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Service.Plugins;

public class PluginRepository : IPluginRepository
{
    private readonly ConcurrentDictionary<string, Plugin> _plugins = new();
    private readonly ConcurrentBag<ServiceDescriptor> _pluginServiceCollection = [];

    public void UnRegisterPlugin(string pluginId, string pluginVersion)
    {
        _plugins.Remove(pluginId, out _);
    }

    public void RegisterPlugin(Plugin plugin, Type @interface, Type concrete)
    {
        RegisterPlugin(plugin);

        _pluginServiceCollection.Add(
            new ServiceDescriptor(@interface, concrete, ServiceLifetime.Singleton)
        );
    }

    public void RegisterPlugin(Plugin plugin)
    {
        _plugins.AddOrUpdate(
            plugin.Id,
            plugin,
            (_, manifest) => manifest.Version < plugin.Version ? plugin : manifest
        );
    }

    public TService? GetPlugin<TService>(
        Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null
    )
        where TService : IPlugin
    {
        return GetPlugins<TService>().FirstOrDefault(filter);
    }

    public IEnumerable<TService> GetPlugins<TService>(
        IEnumerable<(Type @interface, object instance)>? services = null
    )
        where TService : IPlugin
    {
        var serviceCollection = new ServiceCollection { _pluginServiceCollection };

        if (services is not null)
            serviceCollection.Add(
                services.Select(s => new ServiceDescriptor(s.@interface, s.instance))
            );

        return serviceCollection.BuildServiceProvider().GetServices<TService>();
    }

    public IEnumerable<PluginManifest> GetLoadedPluginsManifests()
    {
        return GetLoadedPlugins().Select(plugin => plugin.Manifest);
    }

    public IEnumerable<Plugin> GetLoadedPlugins(string kind)
    {
        return GetLoadedPlugins()
            .Where(plugin =>
                plugin.Manifest.ExtensionPoints.Any(extensionPoint => extensionPoint.Kind == kind)
            );
    }

    public IEnumerable<T> GetLoadedPluginExtensionPoints<T>()
        where T : PluginExtensionPoint
    {
        return GetLoadedPlugins().SelectMany(p => p.Manifest.ExtensionPoints).OfType<T>();
    }

    public Version? GetInstalledPluginVersion(string pluginId)
    {
        return _plugins.TryGetValue(pluginId, out var plugin) ? plugin.Version : null;
    }

    private IEnumerable<Plugin> GetLoadedPlugins()
    {
        return _plugins.Values;
    }
}

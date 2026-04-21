using System.Collections.Concurrent;
using System.Runtime.Loader;
using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Loader;

internal class PluginRepository : IPluginRepository, IPluginRegistry
{
    private readonly ConcurrentDictionary<string, Plugin> _plugins = new();
    private readonly ConcurrentDictionary<string, List<ServiceDescriptor>> _pluginServices = new();
    private readonly ConcurrentDictionary<string, AssemblyLoadContext> _pluginLoadContexts = new();

    public void UnRegisterPlugin(string pluginId)
    {
        if (!_plugins.TryRemove(pluginId, out _))
        {
            return;
        }

        _pluginServices.TryRemove(pluginId, out _);

        if (_pluginLoadContexts.TryRemove(pluginId, out var context))
        {
            context.Unload();
        }
    }

    public void RegisterPlugin(Plugin plugin, LoadedPlugin loadedPlugin)
    {
        RegisterPlugin(plugin);

        var pluginName = plugin.Id.Name;

        _pluginLoadContexts.TryAdd(pluginName, loadedPlugin.LoadContext);

        foreach (var (@interface, concrete) in loadedPlugin.LoadedTypes)
        {
            _pluginServices.AddOrUpdate(
                pluginName,
                [new ServiceDescriptor(@interface, concrete, ServiceLifetime.Singleton)],
                (_, list) =>
                {
                    list.Add(
                        new ServiceDescriptor(@interface, concrete, ServiceLifetime.Singleton)
                    );
                    return list;
                }
            );
        }
    }

    public void RegisterPlugin(Plugin plugin)
    {
        _plugins.AddOrUpdate(
            plugin.Id.Name,
            plugin,
            (_, manifest) => manifest.Id.Version < plugin.Id.Version ? plugin : manifest
        );
    }

    public void UnRegisterPlugin(PluginId pluginId)
    {
        if (!_plugins.TryRemove(pluginId.Name, out _))
        {
            return;
        }
        _pluginServices.TryRemove(pluginId.Name, out _);

        if (_pluginLoadContexts.TryRemove(pluginId.Name, out var context))
        {
            context.Unload();
        }
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
        var serviceCollection = new ServiceCollection();

        foreach (var serviceDescriptors in _pluginServices.Values)
        {
            foreach (var descriptor in serviceDescriptors)
            {
                serviceCollection.Add(descriptor);
            }
        }

        if (services is not null)
        {
            serviceCollection.Add(
                services.Select(s => new ServiceDescriptor(s.@interface, s.instance))
            );
        }

        return serviceCollection.BuildServiceProvider().GetServices<TService>();
    }

    public IEnumerable<Plugin> GetLoadedPlugins()
    {
        return _plugins.Values;
    }
}

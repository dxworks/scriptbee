using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ScriptBee.Plugin;

public class PluginRepository : IPluginRepository
{
    private readonly ConcurrentBag<object> _plugins = new();

    public void RegisterPlugin(object plugin)
    {
        _plugins.Add(plugin);
    }

    public T? GetPlugin<T>(Expression<Func<T, bool>> filter) where T : class
    {
        return GetPlugins(filter).FirstOrDefault();
    }

    public IEnumerable<T> GetPlugins<T>(Expression<Func<T, bool>> filter) where T : class
    {
        return _plugins.OfType<T>().Where(filter.Compile());
    }
}

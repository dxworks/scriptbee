using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ScriptBee.Plugin;

// todo maybe combine with plugin service
public interface IPluginRepository
{
    void RegisterPlugin(object plugin);

    T? GetPlugin<T>(Expression<Func<T, bool>> filter) where T : class;

    IEnumerable<T> GetPlugins<T>(Expression<Func<T, bool>> filter) where T : class;
}

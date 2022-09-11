using System;
using System.Collections.Concurrent;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;

namespace ScriptBee.Plugin;

public class CompositePluginRepository : IPluginRepository
{
    private readonly ConcurrentDictionary<Type, IPluginRepository> _repositories = new();


    public void RegisterPlugin<T>(object argument)
    {
        // _repositories.AddOrUpdate(typeof(T),)
        // _repositories.GetOrAdd(typeof(T), type => new PluginRepository<T>()).RegisterPlugin(argument);
    }

    public T? GetPlugin<T>(object argument)
    {
        throw new System.NotImplementedException();
    }
}

public class ScriptGenerationPluginRepository : IPluginRepository
{
    private readonly ConcurrentDictionary<Type, IScriptGeneratorStrategy> _scriptGeneratorStrategies = new();

    public void RegisterPlugin<T>(object argument)
    {
        throw new NotImplementedException();
    }

    public T? GetPlugin<T>(object argument)
    {
        throw new NotImplementedException();
    }
}

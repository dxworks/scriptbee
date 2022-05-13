using System;
using System.Collections.Generic;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.PluginManager;

public class LinkersHolder : ILinkersHolder
{
    private readonly Dictionary<string, IModelLinker> _linkers = new();

    public void AddLinkerToDictionary(IModelLinker linker)
    {
        var linkerName = linker.GetName();
            
        if (!_linkers.ContainsKey(linkerName))
        {
            _linkers.Add(linkerName,linker);
        }
        else
        {
            Console.Error.WriteLine($"ModelLinker with key {linkerName} already exists");
        }
    }

    public IModelLinker? GetModelLinker(string linkerName)
    {
        return _linkers.TryGetValue(linkerName, out var linker) ? linker : null;
    }

    public IEnumerable<IModelLinker> GetAllLinkers()
    {
        return _linkers.Select(pair => pair.Value);
    }
}
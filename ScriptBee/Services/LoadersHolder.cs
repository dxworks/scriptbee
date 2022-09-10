using System;
using System.Collections.Generic;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Services;

public class LoadersHolder : ILoadersHolder
{
    private readonly Dictionary<string, IModelLoader> _loaders = new();

    public void AddLoaderToDictionary(IModelLoader loader)
    {
        string loaderName = loader.GetName();

        if (!_loaders.ContainsKey(loaderName))
        {
            _loaders.Add(loaderName, loader);
        }
        else
        {
            // todo replace with logger
            Console.Error.WriteLine($"ModelLoader with key {loaderName} already exists");
        }
    }

    public IModelLoader? GetModelLoader(string loaderName)
    {
        if (_loaders.TryGetValue(loaderName, out var loader))
        {
            return loader;
        }
        else
        {
            return null;
        }
    }

    public IEnumerable<IModelLoader> GetAllLoaders()
    {
        return _loaders.Select(pair => pair.Value);
    }
}

using System;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin.Manifest;
using ScriptBee.Services;

namespace ScriptBee.Plugin;

public class LoaderPluginLoader : IPluginLoader
{
    private readonly ILoadersHolder _loadersHolder;

    public LoaderPluginLoader(ILoadersHolder loadersHolder)
    {
        _loadersHolder = loadersHolder;
    }

    public void Load(PluginManifest pluginManifest, Type type)
    {
        if (Activator.CreateInstance(type) is IModelLoader modelLoader)
        {
            _loadersHolder.AddLoaderToDictionary(modelLoader);
        }
    }

    
    public string AcceptedPluginKind => PluginTypes.Loader;
    public void Load(Models.Plugin plugin)
    {
        throw new NotImplementedException();
    }
}

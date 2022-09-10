using System;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Services;

namespace ScriptBee.Plugin;

public class LoaderPluginLoader : IPluginLoader
{
    private readonly ILoadersHolder _loadersHolder;

    public LoaderPluginLoader(ILoadersHolder loadersHolder)
    {
        _loadersHolder = loadersHolder;
    }

    public void LoadPlugin(PluginManifest pluginManifest, Type type)
    {
        if (Activator.CreateInstance(type) is IModelLoader modelLoader)
        {
            _loadersHolder.AddLoaderToDictionary(modelLoader);
        }
    }
}

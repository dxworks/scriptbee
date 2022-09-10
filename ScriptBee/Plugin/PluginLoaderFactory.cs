using System;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using ScriptBee.Services;

namespace ScriptBee.Plugin;

public class PluginLoaderFactory : IPluginLoaderFactory
{
    private readonly ILoadersHolder _loadersHolder;
    private readonly ILinkersHolder _linkersHolder;
    private readonly IScriptGeneratorStrategyHolder _scriptGeneratorStrategyHolder;

    public PluginLoaderFactory(ILoadersHolder loadersHolder, ILinkersHolder linkersHolder,
        IScriptGeneratorStrategyHolder scriptGeneratorStrategyHolder)
    {
        _loadersHolder = loadersHolder;
        _linkersHolder = linkersHolder;
        _scriptGeneratorStrategyHolder = scriptGeneratorStrategyHolder;
    }

    public IPluginLoader? GetPluginLoader(Type type)
    {
        if (typeof(IModelLoader).IsAssignableFrom(type))
        {
            return new LoaderPluginLoader(_loadersHolder);
        }

        if (typeof(IModelLinker).IsAssignableFrom(type))
        {
            return new LinkerPluginLoader(_linkersHolder);
        }

        if (typeof(IScriptGeneratorStrategy).IsAssignableFrom(type))
        {
            return new ScriptGeneratorPluginLoader(_scriptGeneratorStrategyHolder);
        }

        return null;
    }
}

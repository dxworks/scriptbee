using System;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using ScriptBee.Plugin.Manifest;
using ScriptBee.Services;

namespace ScriptBee.Plugin;

public class ScriptGeneratorPluginLoader : IPluginLoader
{
    private readonly IScriptGeneratorStrategyHolder _scriptGeneratorStrategyHolder;

    public ScriptGeneratorPluginLoader(IScriptGeneratorStrategyHolder scriptGeneratorStrategyHolder)
    {
        _scriptGeneratorStrategyHolder = scriptGeneratorStrategyHolder;
    }

    public void LoadPlugin(PluginManifest pluginManifest, Type type)
    {
        if (Activator.CreateInstance(type) is IScriptGeneratorStrategy scriptGeneratorStrategy)
        {
            _scriptGeneratorStrategyHolder.AddStrategy(scriptGeneratorStrategy);
        }
    }
}

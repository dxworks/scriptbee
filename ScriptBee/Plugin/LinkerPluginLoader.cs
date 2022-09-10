using System;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Services;

namespace ScriptBee.Plugin;

public class LinkerPluginLoader : IPluginLoader
{
    private readonly ILinkersHolder _linkersHolder;

    public LinkerPluginLoader(ILinkersHolder linkersHolder)
    {
        _linkersHolder = linkersHolder;
    }

    public void LoadPlugin(PluginManifest pluginManifest, Type type)
    {
        if (Activator.CreateInstance(type) is IModelLinker modelLinker)
        {
            _linkersHolder.AddLinkerToDictionary(modelLinker);
        }
    }
}

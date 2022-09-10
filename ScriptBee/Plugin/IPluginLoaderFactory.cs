using System;

namespace ScriptBee.Plugin;

public interface IPluginLoaderFactory
{
    IPluginLoader? GetPluginLoader(Type type);
}

using System;

namespace ScriptBee.Plugin;

public interface IPluginLoader
{
    void LoadPlugin(PluginManifest pluginManifest, Type type);
}

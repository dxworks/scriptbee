using System;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public interface IPluginLoader
{
    void LoadPlugin(PluginManifest pluginManifest, Type type);
}

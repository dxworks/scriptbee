using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBeeWebApp.Services;

public interface IPluginService
{
    IEnumerable<PluginManifest> GetLoadedPlugins();
}

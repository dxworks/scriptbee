using System.Collections.Generic;

namespace ScriptBee.Plugin.Installer;

public interface IBundlePluginUninstaller
{
    List<(string PluginId, string Version)> Uninstall(string pluginId, string version);
}

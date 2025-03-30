namespace ScriptBee.Ports.Plugins.Installer;

public interface IBundlePluginUninstaller
{
    List<(string PluginId, string Version)> Uninstall(string pluginId, string version);
}

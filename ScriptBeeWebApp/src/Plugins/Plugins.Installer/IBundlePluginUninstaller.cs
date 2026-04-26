using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Installer;

public interface IBundlePluginUninstaller
{
    List<PluginId> Uninstall(PluginId pluginId, string pluginFolderPath);
}

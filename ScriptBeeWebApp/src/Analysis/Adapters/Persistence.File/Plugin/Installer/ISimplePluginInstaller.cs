using OneOf;
using ScriptBee.Ports.Plugins.Installer;

namespace ScriptBee.Persistence.File.Plugin.Installer;

public interface ISimplePluginInstaller
{
    Task<OneOf<string, PluginVersionExistsError, PluginInstallationError>> Install(
        string url,
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    );
}

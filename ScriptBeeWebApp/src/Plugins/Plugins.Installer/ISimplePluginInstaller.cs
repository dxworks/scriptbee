using OneOf;

namespace ScriptBee.Plugins.Installer;

public interface ISimplePluginInstaller
{
    Task<OneOf<string, PluginVersionExistsError, PluginInstallationError>> Install(
        string url,
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    );
}

using OneOf;

namespace ScriptBee.Plugins.Installer;

public interface IBundlePluginInstaller
{
    Task<OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>> Install(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    );
}

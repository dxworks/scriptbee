using OneOf;

namespace ScriptBee.Common.Plugins.Installer;

public interface IBundlePluginInstaller
{
    Task<OneOf<List<string>, PluginVersionExistsError, PluginInstallationError>> Install(
        string pluginId,
        string version,
        CancellationToken cancellationToken = default
    );
}

using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Plugins.Installer;

public interface IBundlePluginInstaller
{
    Task<OneOf<List<PluginId>, PluginInstallationError>> Install(
        PluginId pluginId,
        CancellationToken cancellationToken
    );

    Task<OneOf<List<PluginId>, PluginManifestNotFoundError, PluginInstallationError>> Install(
        ProjectId projectId,
        Stream zipStream,
        CancellationToken cancellationToken
    );
}

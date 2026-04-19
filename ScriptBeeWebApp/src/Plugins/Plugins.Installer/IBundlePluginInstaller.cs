using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Installer;

public interface IBundlePluginInstaller
{
    Task<OneOf<List<PluginId>, PluginInstallationError>> Install(
        PluginId pluginId,
        CancellationToken cancellationToken
    );
}

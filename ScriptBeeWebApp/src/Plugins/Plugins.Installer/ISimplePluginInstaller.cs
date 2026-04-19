using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Installer;

public interface ISimplePluginInstaller
{
    Task<OneOf<string, PluginInstallationError>> Install(
        string url,
        PluginId pluginId,
        CancellationToken cancellationToken = default
    );
}

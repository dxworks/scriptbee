using OneOf;
using OneOf.Types;
using ScriptBee.UseCases.Plugin.Errors;

namespace ScriptBee.UseCases.Plugin;

public interface IInstallPluginUseCase
{
    OneOf<Success, InvalidPluginError, PluginInstallationError> InstallPlugin(
        string pluginId,
        string version
    );
}

using OneOf;
using OneOf.Types;
using ScriptBee.UseCases.Analysis.Errors;

namespace ScriptBee.UseCases.Analysis;

public interface IInstallPluginUseCase
{
    OneOf<Success, InvalidPluginError, PluginInstallationError> InstallPlugin(
        string pluginId,
        string version
    );
}

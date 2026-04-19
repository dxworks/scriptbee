using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.UseCases.Analysis.Errors;

namespace ScriptBee.UseCases.Analysis;

public interface IInstallPluginUseCase
{
    OneOf<Success, InvalidPluginError, PluginInstallationError> InstallPlugin(PluginId pluginId);
}

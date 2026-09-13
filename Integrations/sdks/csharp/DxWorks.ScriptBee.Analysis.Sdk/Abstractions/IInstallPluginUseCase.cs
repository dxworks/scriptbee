using DxWorks.ScriptBee.Analysis.Sdk.Abstractions.Errors;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Plugins;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface IInstallPluginUseCase
{
    OneOf<Success, InvalidPluginError, PluginInstallationError> InstallPlugin(PluginId pluginId);
}

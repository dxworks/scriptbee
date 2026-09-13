using ScriptBee.Domain.Model.Plugins;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface IUninstallPluginUseCase
{
    void UninstallPlugin(PluginId pluginId);
}

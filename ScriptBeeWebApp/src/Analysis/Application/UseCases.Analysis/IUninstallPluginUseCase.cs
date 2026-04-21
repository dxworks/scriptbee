using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.UseCases.Analysis;

public interface IUninstallPluginUseCase
{
    void UninstallPlugin(PluginId pluginId);
}

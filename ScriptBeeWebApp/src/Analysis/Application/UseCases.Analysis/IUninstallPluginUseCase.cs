namespace ScriptBee.UseCases.Analysis;

public interface IUninstallPluginUseCase
{
    void UninstallPlugin(string pluginId, string pluginVersion);
}
